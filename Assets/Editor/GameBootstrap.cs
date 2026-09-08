using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Core.Combat;
using ArenaSurvivor.Enemies;
using ArenaSurvivor.Leveling;
using ArenaSurvivor.Managers;
using ArenaSurvivor.Player;
using ArenaSurvivor.Progression;
using ArenaSurvivor.UI;
using ArenaSurvivor.Weapons;
using Object = UnityEngine.Object;

namespace ArenaSurvivor.EditorTools
{
    /// <summary>
    /// Arma de punta a punta todo lo que falta del juego (Pasos 8 a 15): prefabs de
    /// proyectil y gema de XP, assets de datos, spawner de enemigos, spawner de XP,
    /// GameManager, y toda la UI (barra de XP, timer, contador de kills, panel de subida
    /// de nivel, panel de fin de partida). Vive en Assets/Editor, no entra en ningún build.
    ///
    /// Todo objeto que crea se arma DESACTIVADO, se cablea por completo (SerializedObject)
    /// y recién se reactiva al final — así ningún Awake() corre con referencias sin asignar.
    /// </summary>
    public static class GameBootstrap
    {
        [MenuItem("Tools/Arena Survivor/Build Everything")]
        private static void BuildEverything()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("[GameBootstrap] No se encontró el Player en la escena.");
                return;
            }

            var circleSprite = player.GetComponent<SpriteRenderer>().sprite;
            var enemyPrefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/Enemy.prefab");
            if (enemyPrefabAsset == null)
            {
                Debug.LogError("[GameBootstrap] No se encontró Assets/Prefabs/Enemies/Enemy.prefab.");
                return;
            }

            EnsureFolder("Assets/Prefabs/Weapons");
            EnsureFolder("Assets/Prefabs/Progression");
            EnsureFolder("Assets/Data/Weapons");

            // ---- Data assets ----
            var weaponDataMagic = CreateWeaponData("Assets/Data/Weapons/WeaponData_MagicMissile.asset", "Bala Mágica",
                maxLevel: 5, baseDamage: 10f, damagePerLevel: 4f, baseCooldown: 1f, cooldownReductionPerLevel: 0.08f,
                minCooldown: 0.25f, projectileSpeed: 12f, projectileLifetime: 3f);

            var weaponDataOrbital = CreateWeaponData("Assets/Data/Weapons/WeaponData_Orbital.asset", "Orbe Espectral",
                maxLevel: 5, baseDamage: 8f, damagePerLevel: 3f, baseCooldown: 1f, cooldownReductionPerLevel: 0f,
                minCooldown: 1f, projectileSpeed: 1f, projectileLifetime: 1f);

            var enemyDataBasic = AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/Data/Enemies/EnemyData_Basic.asset");
            var enemyDataSwarm = CreateEnemyData("Assets/Data/Enemies/EnemyData_Swarm.asset",
                maxHealth: 8f, moveSpeed: 3.5f, contactDamage: 6f, experienceReward: 3,
                color: new Color(1f, 0.6f, 0.1f), scale: 0.7f);
            var enemyDataTank = CreateEnemyData("Assets/Data/Enemies/EnemyData_Tank.asset",
                maxHealth: 60f, moveSpeed: 1.1f, contactDamage: 18f, experienceReward: 12,
                color: new Color(0.4f, 0.05f, 0.05f), scale: 1.6f);

            // ---- Prefabs ----
            var projectilePrefab = BuildProjectilePrefab(circleSprite);
            var xpGemPrefab = BuildXpGemPrefab(circleSprite);

            // ---- Player: stats, XP, armas ----
            player.SetActive(false);

            var playerStats = EnsureComponent<PlayerStats>(player);
            var playerLeveling = EnsureComponent<PlayerLeveling>(player);
            var playerHealth = player.GetComponent<Health>();

            var projectileWeapon = EnsureComponent<ProjectileWeapon>(player);
            SetField(projectileWeapon, "data", weaponDataMagic);
            SetField(projectileWeapon, "projectilePrefab", projectilePrefab);

            var orbitVisual = BuildOrbitVisual(player.transform, circleSprite);
            var orbitalWeapon = EnsureComponent<OrbitalWeapon>(player);
            SetField(orbitalWeapon, "data", weaponDataOrbital);
            SetField(orbitalWeapon, "orbitVisual", orbitVisual);
            orbitalWeapon.enabled = false;

            // ---- Enemy spawner (reemplaza al DebugEnemySpawnTester) ----
            RemoveGameObjectIfPresent("EnemySpawnTester");

            var spawnerObj = GameObject.Find("EnemySpawner");
            EnemySpawner spawner;
            if (spawnerObj == null)
            {
                var spawnerGO = new GameObject("EnemySpawner");
                spawnerGO.SetActive(false);
                spawner = spawnerGO.AddComponent<EnemySpawner>();
                SetField(spawner, "enemyPrefab", enemyPrefabAsset.GetComponent<Enemy>());
                SetField(spawner, "target", player.transform);
                SetWaveEntries(spawner, "waveEntries",
                    (enemyDataBasic, 0f),
                    (enemyDataSwarm, 20f),
                    (enemyDataTank, 45f));
                spawnerGO.SetActive(true);
            }
            else
            {
                spawner = spawnerObj.GetComponent<EnemySpawner>();
            }

            // ---- XP gem spawner ----
            if (GameObject.Find("XpGemSpawner") == null)
            {
                var xpSpawnerGO = new GameObject("XpGemSpawner");
                xpSpawnerGO.SetActive(false);
                var xpSpawner = xpSpawnerGO.AddComponent<XpGemSpawner>();
                SetField(xpSpawner, "gemPrefab", xpGemPrefab);
                SetField(xpSpawner, "player", player.transform);
                xpSpawnerGO.SetActive(true);
            }

            // ---- GameManager ----
            if (GameObject.Find("GameManager") == null)
            {
                var gameManagerGO = new GameObject("GameManager");
                gameManagerGO.SetActive(false);
                gameManagerGO.AddComponent<GameManager>();
                gameManagerGO.SetActive(true);
            }

            // ---- UI ----
            var canvasGO = GameObject.Find("Canvas");
            if (canvasGO == null)
            {
                Debug.LogError("[GameBootstrap] No se encontró el Canvas en la escena.");
                player.SetActive(true);
                return;
            }

            var canvas = canvasGO.transform;
            var flatSprite = CreateFlatSpriteAsset();

            BuildXpBar(canvas, flatSprite);
            BuildTimerText(canvas);
            BuildKillCounterText(canvas);
            BuildLevelUpPanel(canvas, flatSprite, projectileWeapon, orbitalWeapon, playerStats, playerHealth);
            BuildGameEndPanel(canvas);
            EnsureHealthBarHpText(canvas);
            BuildPlayerStatsText(canvas);
            BuildWeaponsHudText(canvas);
            EnsureHealthBarFillConfigured(canvas, flatSprite);
            EnsureLevelUpButtonSprites(canvas, flatSprite);

            ApplyBalanceTuning(playerHealth, playerLeveling, xpGemPrefab, spawner);
            ApplyPhysicsSmoothing(player, enemyPrefabAsset, projectilePrefab, xpGemPrefab);
            SetField(spawner, "groundTransform", GameObject.Find("Ground")?.transform);
            EnsureGroundPattern();

            player.SetActive(true);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

            Debug.Log("[GameBootstrap] Listo: spawner, armas, XP/nivel, panel de mejoras, HUD y fin de partida armados.");
        }

        // ---------------- Data assets ----------------

        private static WeaponData CreateWeaponData(string path, string weaponName, int maxLevel, float baseDamage,
            float damagePerLevel, float baseCooldown, float cooldownReductionPerLevel, float minCooldown,
            float projectileSpeed, float projectileLifetime)
        {
            var existing = AssetDatabase.LoadAssetAtPath<WeaponData>(path);
            if (existing != null)
            {
                return existing;
            }

            var asset = ScriptableObject.CreateInstance<WeaponData>();
            AssetDatabase.CreateAsset(asset, path);

            SetField(asset, "weaponName", weaponName);
            SetField(asset, "maxLevel", maxLevel);
            SetField(asset, "baseDamage", baseDamage);
            SetField(asset, "damagePerLevel", damagePerLevel);
            SetField(asset, "baseCooldown", baseCooldown);
            SetField(asset, "cooldownReductionPerLevel", cooldownReductionPerLevel);
            SetField(asset, "minCooldown", minCooldown);
            SetField(asset, "projectileSpeed", projectileSpeed);
            SetField(asset, "projectileLifetime", projectileLifetime);

            return asset;
        }

        private static EnemyData CreateEnemyData(string path, float maxHealth, float moveSpeed, float contactDamage,
            int experienceReward, Color color, float scale)
        {
            var existing = AssetDatabase.LoadAssetAtPath<EnemyData>(path);
            if (existing != null)
            {
                return existing;
            }

            var asset = ScriptableObject.CreateInstance<EnemyData>();
            AssetDatabase.CreateAsset(asset, path);

            SetField(asset, "maxHealth", maxHealth);
            SetField(asset, "moveSpeed", moveSpeed);
            SetField(asset, "contactDamage", contactDamage);
            SetField(asset, "experienceReward", experienceReward);
            SetField(asset, "color", color);
            SetField(asset, "scale", scale);

            return asset;
        }

        // ---------------- Prefabs ----------------

        private static Projectile BuildProjectilePrefab(Sprite circleSprite)
        {
            const string path = "Assets/Prefabs/Weapons/Projectile.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<Projectile>(path);
            if (existing != null)
            {
                return existing;
            }

            var go = new GameObject("Projectile");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = circleSprite;
            sr.color = new Color(1f, 0.9f, 0.2f);
            go.transform.localScale = Vector3.one * 0.3f;

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            go.AddComponent<Projectile>();

            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return savedPrefab.GetComponent<Projectile>();
        }

        private static XpGem BuildXpGemPrefab(Sprite circleSprite)
        {
            const string path = "Assets/Prefabs/Progression/XpGem.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<XpGem>(path);
            if (existing != null)
            {
                return existing;
            }

            var go = new GameObject("XpGem");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = circleSprite;
            sr.color = new Color(0.2f, 0.9f, 1f);
            go.transform.localScale = Vector3.one * 0.25f;

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            go.AddComponent<XpGem>();

            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return savedPrefab.GetComponent<XpGem>();
        }

        private static Transform BuildOrbitVisual(Transform playerTransform, Sprite circleSprite)
        {
            var existing = playerTransform.Find("OrbitVisual");
            if (existing != null)
            {
                return existing;
            }

            var go = new GameObject("OrbitVisual");
            go.transform.SetParent(playerTransform, false);
            go.transform.localScale = Vector3.one * 0.4f;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = circleSprite;
            sr.color = new Color(0.7f, 0.3f, 1f);

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            go.AddComponent<OrbitalHitbox>();

            return go.transform;
        }

        // ---------------- UI ----------------

        private static void BuildXpBar(Transform canvas, Sprite uiSprite)
        {
            if (canvas.Find("XpBar_Background") != null)
            {
                return;
            }

            var backgroundGO = new GameObject("XpBar_Background", typeof(RectTransform));
            backgroundGO.SetActive(false);
            backgroundGO.transform.SetParent(canvas, false);

            var background = backgroundGO.AddComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.6f);
            SetRect(background.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(40, -110), new Vector2(500, 26));

            var fill = CreateImage("XpBar_Fill", background.transform, new Color(0.3f, 0.6f, 1f), uiSprite);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = (int)Image.OriginHorizontal.Left;
            fill.fillAmount = 0f;
            StretchFill(fill.rectTransform);

            var levelText = CreateText("XpBar_LevelText", canvas, "Nv. 1", 26, Color.white, TextAnchor.MiddleLeft);
            SetRect(levelText.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(550, -108), new Vector2(150, 30));

            var xpBarUI = backgroundGO.AddComponent<XpBarUI>();
            SetField(xpBarUI, "fillImage", fill);
            SetField(xpBarUI, "levelText", levelText);

            backgroundGO.SetActive(true);
        }

        private static void BuildTimerText(Transform canvas)
        {
            if (canvas.Find("Timer_Text") != null)
            {
                return;
            }

            var timerGO = new GameObject("Timer_Text", typeof(RectTransform));
            timerGO.SetActive(false);
            timerGO.transform.SetParent(canvas, false);

            var timerText = timerGO.AddComponent<Text>();
            ConfigureText(timerText, "00:00", 40, Color.white, TextAnchor.MiddleCenter);
            SetRect(timerText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0, -40), new Vector2(220, 60));

            var timerUI = timerGO.AddComponent<GameTimerUI>();
            SetField(timerUI, "timerText", timerText);

            timerGO.SetActive(true);
        }

        private static void BuildKillCounterText(Transform canvas)
        {
            if (canvas.Find("KillCounter_Text") != null)
            {
                return;
            }

            var killGO = new GameObject("KillCounter_Text", typeof(RectTransform));
            killGO.SetActive(false);
            killGO.transform.SetParent(canvas, false);

            var killText = killGO.AddComponent<Text>();
            ConfigureText(killText, "Kills: 0", 28, Color.white, TextAnchor.MiddleRight);
            SetRect(killText.rectTransform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1),
                new Vector2(-40, -40), new Vector2(260, 50));

            var killUI = killGO.AddComponent<KillCounterUI>();
            SetField(killUI, "countText", killText);

            killGO.SetActive(true);
        }

        private static void BuildLevelUpPanel(Transform canvas, Sprite uiSprite, ProjectileWeapon projectileWeapon,
            OrbitalWeapon orbitalWeapon, PlayerStats playerStats, Health playerHealth)
        {
            if (canvas.Find("LevelUpPanel") != null)
            {
                return;
            }

            var panelGO = new GameObject("LevelUpPanel", typeof(RectTransform));
            panelGO.SetActive(false);
            panelGO.transform.SetParent(canvas, false);

            var dim = panelGO.AddComponent<Image>();
            dim.color = new Color(0f, 0f, 0f, 0.75f);
            StretchFill(dim.rectTransform);

            var title = CreateText("Title", panelGO.transform, "¡Subiste de nivel!", 48, Color.white, TextAnchor.MiddleCenter);
            SetRect(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0, 220), new Vector2(800, 80));

            var buttons = new Object[3];
            for (var i = 0; i < 3; i++)
            {
                var button = CreateButton($"Option_{i}", panelGO.transform, "Opción", uiSprite);
                SetRect(button.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f), new Vector2(0, 60 - i * 130), new Vector2(600, 100));

                var buttonUI = button.gameObject.AddComponent<LevelUpButtonUI>();
                SetField(buttonUI, "button", button);
                SetField(buttonUI, "titleText", button.GetComponentInChildren<Text>());
                buttons[i] = buttonUI;
            }

            // El controller vive en el Canvas (siempre activo), NO en el panel: si viviera en
            // panelGO, al arrancar con el panel oculto su propio OnEnable nunca correría y
            // jamás se suscribiría a PlayerLeveledUpEvent.
            var manager = canvas.gameObject.AddComponent<LevelUpManager>();
            SetField(manager, "panelRoot", panelGO);
            SetObjectArray(manager, "buttons", buttons);
            SetObjectArray(manager, "weapons", new Object[] { projectileWeapon, orbitalWeapon });
            SetField(manager, "playerStats", playerStats);
            SetField(manager, "playerHealth", playerHealth);

            panelGO.SetActive(true);
        }

        private static void BuildGameEndPanel(Transform canvas)
        {
            if (canvas.Find("GameEndPanel") != null)
            {
                return;
            }

            var panelGO = new GameObject("GameEndPanel", typeof(RectTransform));
            panelGO.SetActive(false);
            panelGO.transform.SetParent(canvas, false);

            var dim = panelGO.AddComponent<Image>();
            dim.color = new Color(0f, 0f, 0f, 0.85f);
            StretchFill(dim.rectTransform);

            var message = CreateText("Message", panelGO.transform, "GAME OVER", 52, Color.white, TextAnchor.MiddleCenter);
            StretchFill(message.rectTransform);

            // Mismo motivo que en LevelUpPanel: el controller va en el Canvas, no en el panel.
            var endUI = canvas.gameObject.AddComponent<GameEndUI>();
            SetField(endUI, "panelRoot", panelGO);
            SetField(endUI, "messageText", message);

            panelGO.SetActive(true);
        }

        private static void EnsureHealthBarFillConfigured(Transform canvas, Sprite flatSprite)
        {
            var fillTransform = canvas.Find("HealthBar_Background/HealthBar_Fill");
            if (fillTransform == null)
            {
                Debug.LogWarning("[GameBootstrap] No se encontró HealthBar_Fill para reconfigurar.");
                return;
            }

            // Se reaplica siempre (no solo si falta): en algún momento este Image quedó con
            // Source Image = None y Type = Simple, lo que hace que fillAmount no tenga ningún
            // efecto visual — la barra se ve siempre llena sin importar la vida real. Además
            // usamos un sprite plano (no UISprite) porque un sprite con bordes/esquinas
            // redondeadas se deforma al estirarlo en una barra angosta.
            var fillImage = fillTransform.GetComponent<Image>();
            fillImage.sprite = flatSprite;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;

            var xpFillTransform = canvas.Find("XpBar_Background/XpBar_Fill");
            if (xpFillTransform != null)
            {
                xpFillTransform.GetComponent<Image>().sprite = flatSprite;
            }
        }

        private static void EnsureLevelUpButtonSprites(Transform canvas, Sprite flatSprite)
        {
            var panel = canvas.Find("LevelUpPanel");
            if (panel == null)
            {
                return;
            }

            for (var i = 0; i < 3; i++)
            {
                var option = panel.Find($"Option_{i}");
                if (option != null)
                {
                    option.GetComponent<Image>().sprite = flatSprite;
                }
            }
        }

        /// <summary>
        /// Un cuadrado blanco liso de 4x4, guardado como asset real (no un Sprite temporal en
        /// memoria, que se perdería al recargar el dominio). Lo usamos en vez de los sprites
        /// de UI de Unity porque esos tienen bordes/esquinas pensados para botones cuadrados:
        /// al estirarlos en una barra angosta (500x26) se ven deformados.
        /// </summary>
        private static Sprite CreateFlatSpriteAsset()
        {
            const string path = "Assets/Data/UI/FlatWhite.png";
            var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null)
            {
                return existing;
            }

            EnsureFolder("Assets/Data/UI");

            var texture = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            var pixels = new Color32[16];
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color32(255, 255, 255, 255);
            }

            texture.SetPixels32(pixels);
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);

            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static void ApplyPhysicsSmoothing(GameObject player, GameObject enemyPrefabAsset, Projectile projectilePrefab, XpGem xpGemPrefab)
        {
            // El titileo al moverse es el clásico problema de mover un Rigidbody2D por física
            // (FixedUpdate, a tasa fija) mientras la cámara y el render van a la tasa de
            // pantalla (variable) — Interpolate le pide a Unity que suavice la posición
            // RENDERIZADA entre pasos de física, sin tocar la simulación en sí.
            SetInterpolation(player.GetComponent<Rigidbody2D>());
            SetInterpolation(enemyPrefabAsset.GetComponent<Rigidbody2D>());
            SetInterpolation(projectilePrefab.GetComponent<Rigidbody2D>());
            SetInterpolation(xpGemPrefab.GetComponent<Rigidbody2D>());
        }

        private static void SetInterpolation(Rigidbody2D rb)
        {
            if (rb == null)
            {
                return;
            }

            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            // Los prefabs (enemigo, proyectil, gema) no son objetos de escena: sin esto, el
            // cambio puede no quedar guardado en el asset al hacer AssetDatabase.SaveAssets().
            EditorUtility.SetDirty(rb);
            EditorUtility.SetDirty(rb.gameObject);
        }

        private static void EnsureGroundPattern()
        {
            // El patrón de suelo (Assets/Sprites/PatronGround.jpeg) se aplica con
            // SpriteRenderer.Draw Mode = Tiled: Unity repite el sprite en mosaico dentro del
            // tamaño real del Ground sin deformarlo (a diferencia de tocar el Tiling de un
            // Material, que Unity marca como incompatible con SpriteRenderer). El color del
            // Ground sigue multiplicando al patrón normalmente (tinte propio de SpriteRenderer),
            // así que no hace falta un shader especial. La densidad del mosaico se controla
            // 100% desde el import setting "Pixels Per Unit" del sprite, no desde acá.
            var groundGO = GameObject.Find("Ground");
            if (groundGO == null)
            {
                return;
            }

            var renderer = groundGO.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                return;
            }

            var patternSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/PatronGround.jpeg");
            var groundMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Sprites/Ground MAT.mat");

            if (patternSprite != null)
            {
                renderer.sprite = patternSprite;
            }

            if (groundMaterial != null)
            {
                renderer.sharedMaterial = groundMaterial;
            }

            renderer.drawMode = SpriteDrawMode.Tiled;
            renderer.size = Vector2.one;

            EditorUtility.SetDirty(renderer);
        }

        private static void EnsureHealthBarHpText(Transform canvas)
        {
            var background = canvas.Find("HealthBar_Background");
            var hudGO = GameObject.Find("HUD");
            if (background == null || hudGO == null || background.Find("HealthBar_HpText") != null)
            {
                return;
            }

            var hpText = CreateText("HealthBar_HpText", background, "100/100", 20, Color.white, TextAnchor.MiddleCenter);
            StretchFill(hpText.rectTransform);

            var healthBarUI = hudGO.GetComponent<HealthBarUI>();
            SetField(healthBarUI, "hpText", hpText);
        }

        private static void BuildPlayerStatsText(Transform canvas)
        {
            if (canvas.Find("PlayerStats_Text") != null)
            {
                return;
            }

            var statsGO = new GameObject("PlayerStats_Text", typeof(RectTransform));
            statsGO.SetActive(false);
            statsGO.transform.SetParent(canvas, false);

            var statsText = statsGO.AddComponent<Text>();
            ConfigureText(statsText, "Velocidad: 100%   Daño: 100%", 22, Color.white, TextAnchor.MiddleLeft);
            SetRect(statsText.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(40, -150), new Vector2(500, 30));

            var statsUI = statsGO.AddComponent<PlayerStatsUI>();
            SetField(statsUI, "statsText", statsText);

            statsGO.SetActive(true);
        }

        private static void BuildWeaponsHudText(Transform canvas)
        {
            if (canvas.Find("Weapons_Text") != null)
            {
                return;
            }

            var weaponsGO = new GameObject("Weapons_Text", typeof(RectTransform));
            weaponsGO.SetActive(false);
            weaponsGO.transform.SetParent(canvas, false);

            var weaponsText = weaponsGO.AddComponent<Text>();
            ConfigureText(weaponsText, string.Empty, 22, Color.white, TextAnchor.UpperLeft);
            SetRect(weaponsText.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(40, -190), new Vector2(500, 130));

            var weaponsUI = weaponsGO.AddComponent<WeaponsHudUI>();
            SetField(weaponsUI, "weaponsText", weaponsText);

            weaponsGO.SetActive(true);
        }

        private static void ApplyBalanceTuning(Health playerHealth, PlayerLeveling playerLeveling, XpGem xpGemPrefab, EnemySpawner spawner)
        {
            SetField(playerHealth, "invulnerabilityDuration", 0.5f);

            SetField(playerLeveling, "baseXpToLevel", 25);
            SetField(playerLeveling, "xpCurveMultiplier", 1.35f);

            SetField(xpGemPrefab, "magnetRadius", 5f);
            SetField(xpGemPrefab, "magnetSpeed", 12f);

            SetField(spawner, "spawnRadiusMin", 18f);
            SetField(spawner, "spawnRadiusMax", 24f);
        }

        // ---------------- UI helpers ----------------

        private static void SetRect(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
            Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = sizeDelta;
        }

        private static void StretchFill(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static Image CreateImage(string name, Transform parent, Color color, Sprite sprite)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var image = go.AddComponent<Image>();
            image.color = color;
            if (sprite != null)
            {
                image.sprite = sprite;
            }

            return image;
        }

        private static void ConfigureText(Text text, string content, int fontSize, Color color, TextAnchor alignment)
        {
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
        }

        private static Text CreateText(string name, Transform parent, string content, int fontSize, Color color, TextAnchor alignment)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<Text>();
            ConfigureText(text, content, fontSize, color, alignment);
            return text;
        }

        private static Button CreateButton(string name, Transform parent, string label, Sprite uiSprite)
        {
            var image = CreateImage(name, parent, new Color(0.12f, 0.12f, 0.12f, 0.95f), uiSprite);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var text = CreateText("Label", image.transform, label, 26, Color.white, TextAnchor.MiddleCenter);
            StretchFill(text.rectTransform);

            return button;
        }

        // ---------------- Utilidades genéricas ----------------

        private static T EnsureComponent<T>(GameObject go) where T : Component
        {
            var existing = go.GetComponent<T>();
            return existing != null ? existing : go.AddComponent<T>();
        }

        private static void RemoveGameObjectIfPresent(string objectName)
        {
            var go = GameObject.Find(objectName);
            if (go != null)
            {
                Object.DestroyImmediate(go);
            }
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
            var folderName = Path.GetFileName(path);

            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent, folderName);
        }

        private static void SetField(Object target, string fieldName, object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop == null)
            {
                Debug.LogError($"[GameBootstrap] Campo '{fieldName}' no encontrado en {target.GetType().Name}.");
                return;
            }

            switch (value)
            {
                case null:
                    prop.objectReferenceValue = null;
                    break;
                case Object o:
                    prop.objectReferenceValue = o;
                    break;
                case float f:
                    prop.floatValue = f;
                    break;
                case int i:
                    prop.intValue = i;
                    break;
                case bool b:
                    prop.boolValue = b;
                    break;
                case string s:
                    prop.stringValue = s;
                    break;
                case Color c:
                    prop.colorValue = c;
                    break;
                default:
                    Debug.LogError($"[GameBootstrap] Tipo no soportado para '{fieldName}': {value.GetType()}");
                    break;
            }

            so.ApplyModifiedProperties();
        }

        private static void SetObjectArray(Object target, string fieldName, Object[] values)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            prop.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
            {
                prop.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }

            so.ApplyModifiedProperties();
        }

        private static void SetWaveEntries(EnemySpawner spawner, string fieldName, params (EnemyData data, float unlockTime)[] entries)
        {
            var so = new SerializedObject(spawner);
            var prop = so.FindProperty(fieldName);
            prop.arraySize = entries.Length;

            for (var i = 0; i < entries.Length; i++)
            {
                var element = prop.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("Data").objectReferenceValue = entries[i].data;
                element.FindPropertyRelative("UnlockTimeSeconds").floatValue = entries[i].unlockTime;
            }

            so.ApplyModifiedProperties();
        }
    }
}
