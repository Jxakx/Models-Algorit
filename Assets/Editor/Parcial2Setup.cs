using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Audio;
using ArenaSurvivor.Core.Audio;
using ArenaSurvivor.Core.Combat;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Enemies;
using ArenaSurvivor.Enemies.Movement;
using ArenaSurvivor.Fx;
using ArenaSurvivor.Leveling;
using ArenaSurvivor.Localization;
using ArenaSurvivor.Managers;
using ArenaSurvivor.Player;
using ArenaSurvivor.UI;
using ArenaSurvivor.Weapons;
using ArenaSurvivor.Weapons.Targeting;
using Object = UnityEngine.Object;

namespace ArenaSurvivor.EditorTools
{
    /// <summary>
    /// Prepara la escena "MUNDO 1" para los patrones del Parcial 2: crea los assets de datos
    /// (estrategias, estilos, tabla de textos), arma las armas nuevas, el audio, el FX, el
    /// menú de pausa, el pantallazo azul y el panel de Flyweight, y los cablea con
    /// SerializedObject. Es idempotente: se puede correr varias veces sin duplicar nada ni
    /// pisar lo que se haya retocado a mano. Vive en Assets/Editor, no entra en ningún build.
    /// </summary>
    public static class Parcial2Setup
    {
        private const string ScenePath = "Assets/Scenes/MUNDO 1.unity";

        [MenuItem("Tools/Arena Survivor/Parcial 2/Preparar escena MUNDO 1")]
        private static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("[Parcial2Setup] Salí de Play antes de preparar la escena.");
                return;
            }

            var scene = EditorSceneManager.GetActiveScene();
            if (scene.path != ScenePath)
            {
                scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            }

            var player = GameObject.FindGameObjectWithTag("Player");
            var canvasGO = GameObject.Find("Canvas");
            var gameManagerGO = GameObject.Find("GameManager");
            var enemyPrefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/Enemy.prefab");
            var projectilePrefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Weapons/Projectile.prefab");
            var flatSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Data/UI/FlatWhite.png");

            if (player == null || canvasGO == null || gameManagerGO == null || enemyPrefabAsset == null
                || projectilePrefabAsset == null)
            {
                Debug.LogError("[Parcial2Setup] Falta el Player, el Canvas, el GameManager o un prefab base en MUNDO 1.");
                return;
            }

            var circleSprite = player.GetComponent<SpriteRenderer>().sprite;
            var canvas = canvasGO.transform;

            EnsureFolder("Assets/Data/Strategies/Targeting");
            EnsureFolder("Assets/Data/Strategies/Movement");
            EnsureFolder("Assets/Data/Fx");
            EnsureFolder("Assets/Data/Localization");
            EnsureFolder("Assets/Prefabs/Fx");

            // ---- Strategy: assets de targeting y de movimiento ----
            var nearest = LoadOrCreate<NearestTargeting>("Assets/Data/Strategies/Targeting/Targeting_Nearest.asset");
            var farthest = LoadOrCreate<FarthestTargeting>("Assets/Data/Strategies/Targeting/Targeting_Farthest.asset");
            var strongest = LoadOrCreate<StrongestTargeting>("Assets/Data/Strategies/Targeting/Targeting_Strongest.asset");
            LoadOrCreate<RandomTargeting>("Assets/Data/Strategies/Targeting/Targeting_Random.asset");

            var chase = LoadOrCreate<ChaseMovement>("Assets/Data/Strategies/Movement/Movement_Chase.asset");
            var zigzag = LoadOrCreate<ZigZagMovement>("Assets/Data/Strategies/Movement/Movement_ZigZag.asset");
            var charge = LoadOrCreate<ChargeMovement>("Assets/Data/Strategies/Movement/Movement_Charge.asset");

            // Un movimiento distinto por tipo de enemigo (solo si todavía no tienen uno elegido).
            AssignMovementIfEmpty("Assets/Data/Enemies/EnemyData_Basic.asset", chase);
            AssignMovementIfEmpty("Assets/Data/Enemies/EnemyData_Swarm.asset", zigzag);
            AssignMovementIfEmpty("Assets/Data/Enemies/EnemyData_Tank.asset", charge);

            // ---- Flyweight: estilos compartidos de los números de daño ----
            var enemyDamageStyle = LoadOrCreate<DamageNumberStyle>("Assets/Data/Fx/DamageStyle_Enemy.asset", style =>
            {
                SetField(style, "color", Color.white);
                SetField(style, "moveSpeed", 1.2f);
                SetField(style, "lifetime", 0.6f);
            });
            var playerDamageStyle = LoadOrCreate<DamageNumberStyle>("Assets/Data/Fx/DamageStyle_Player.asset", style =>
            {
                SetField(style, "color", new Color(1f, 0.25f, 0.25f));
                SetField(style, "moveSpeed", 1.4f);
                SetField(style, "lifetime", 0.85f);
            });

            // ---- Datos de armas (con su Look-Up Table por nivel) ----
            var magicData = ConfigureExistingWeaponData("Assets/Data/Weapons/WeaponData_MagicMissile.asset",
                LocKeys.WeaponMagicMissile, SoundId.Shoot);
            var orbitalData = ConfigureExistingWeaponData("Assets/Data/Weapons/WeaponData_Orbital.asset",
                LocKeys.WeaponOrbital, SoundId.Shoot);
            var laserData = EnsureWeaponData("Assets/Data/Weapons/WeaponData_Laser.asset", "Haz Perforante",
                LocKeys.WeaponLaser, SoundId.Laser, 5, 6f, 3f, 1.8f, 0.1f, 0.55f, 0f, 0f);
            var chainData = EnsureWeaponData("Assets/Data/Weapons/WeaponData_Chain.asset", "Cadena Eléctrica",
                LocKeys.WeaponChain, SoundId.ChainLightning, 5, 7f, 3f, 1.4f, 0.08f, 0.45f, 0f, 0f);
            var droneData = EnsureWeaponData("Assets/Data/Weapons/WeaponData_Drone.asset", "Dron Acompañante",
                LocKeys.WeaponDrone, SoundId.Shoot, 5, 6f, 2f, 1.2f, 0.05f, 0.5f, 10f, 3f);

            // ---- Prefabs de FX (partículas y número de daño) ----
            // El material de las partículas tiene que ser un ASSET guardado: un Material creado
            // en memoria no se serializa dentro de un prefab y las partículas salían magenta.
            var particleMaterial = EnsureParticleMaterial();
            var damageNumberPrefab = BuildDamageNumberPrefab();
            var deathBurstPrefab = BuildParticleBurstPrefab("EnemyDeathBurst", "Assets/Prefabs/Fx/EnemyDeathBurst.prefab",
                16, 3.5f, 0.18f, 0.4f, 0.1f, particleMaterial);
            var impactBurstPrefab = BuildParticleBurstPrefab("ImpactBurst", "Assets/Prefabs/Fx/ImpactBurst.prefab",
                6, 2.5f, 0.1f, 0.2f, 0.03f, particleMaterial);

            // Enemigo: parpadeo al recibir daño, "pop" al aparecer y número de daño.
            ConfigureEnemyPrefab(enemyDamageStyle);

            // ---- Player: armas, feedback ----
            var playerHealth = player.GetComponent<Health>();
            var playerStats = player.GetComponent<PlayerStats>();
            var playerLeveling = player.GetComponent<PlayerLeveling>();
            var projectileWeapon = player.GetComponent<ProjectileWeapon>();
            var orbitalWeapon = player.GetComponent<OrbitalWeapon>();

            EnsureComponent<HitFlash>(player);
            var playerNumbers = EnsureComponent<DamageNumberSpawner>(player);
            SetField(playerNumbers, "style", playerDamageStyle);

            SetField(projectileWeapon, "targetingStrategy", nearest);
            SetField(orbitalWeapon, "knockbackForce", 4f);

            // Las armas nuevas arrancan desactivadas: se desbloquean desde el panel de nivel.
            var laserWeapon = EnsureComponent<LaserWeapon>(player, out var laserCreated);
            var chainWeapon = EnsureComponent<ChainLightningWeapon>(player, out var chainCreated);
            var droneWeapon = EnsureComponent<DroneWeapon>(player, out var droneCreated);

            SetField(laserWeapon, "data", laserData);
            SetField(chainWeapon, "data", chainData);
            SetField(droneWeapon, "data", droneData);

            SetField(laserWeapon, "beamVisual", BuildLineVisual(player.transform, "LaserBeamVisual", new Color(0.4f, 1f, 1f), 0.12f));
            SetField(chainWeapon, "chainVisual", BuildLineVisual(player.transform, "ChainVisual", new Color(0.6f, 0.8f, 1f), 0.08f));
            SetField(droneWeapon, "droneVisual", BuildDroneVisual(player.transform, circleSprite));
            SetField(droneWeapon, "projectilePrefab", projectilePrefabAsset.GetComponent<Projectile>());

            if (laserCreated)
            {
                SetField(laserWeapon, "targetingStrategy", strongest);
                SetField(laserWeapon, "knockbackForce", 2f);
                SetField(laserWeapon, "slowMultiplier", 0.5f);
                SetField(laserWeapon, "slowDuration", 1.5f);
                laserWeapon.enabled = false;
            }

            if (chainCreated)
            {
                SetField(chainWeapon, "targetingStrategy", nearest);
                SetField(chainWeapon, "knockbackForce", 2f);
                chainWeapon.enabled = false;
            }

            if (droneCreated)
            {
                SetField(droneWeapon, "targetingStrategy", farthest);
                SetField(droneWeapon, "knockbackForce", 1.5f);
                droneWeapon.enabled = false;
            }

            // Los visuales de las armas bloqueadas no se ven hasta que el arma se habilita.
            SetVisualActive(player.transform, "OrbitVisual", orbitalWeapon.enabled);
            SetVisualActive(player.transform, "DroneVisual", droneWeapon.enabled);

            // ---- Look-Up Table de XP ----
            if (new SerializedObject(playerLeveling).FindProperty("xpPerLevel").arraySize == 0)
            {
                playerLeveling.RebuildXpTable();
                EditorUtility.SetDirty(playerLeveling);
            }

            // ---- Panel de nivel: las 5 armas ----
            var levelUpManager = canvasGO.GetComponent<LevelUpManager>();
            SetObjectArray(levelUpManager, "weapons",
                new Object[] { projectileWeapon, orbitalWeapon, laserWeapon, chainWeapon, droneWeapon });
            SetField(levelUpManager, "playerStats", playerStats);
            SetField(levelUpManager, "playerHealth", playerHealth);
            AddLocalizedText(canvas.Find("LevelUpPanel/Title"), LocKeys.LevelUpTitle);

            // ---- Localization ----
            var localizationTable = LoadOrCreate<LocalizationTable>("Assets/Data/Localization/LocalizationTable.asset");
            localizationTable.MergeDefaults();
            EditorUtility.SetDirty(localizationTable);

            var localizationGO = FindOrCreate("Localization");
            var localizationManager = EnsureComponent<LocalizationManager>(localizationGO);
            SetField(localizationManager, "table", localizationTable);

            // ---- Audio: Look-Up Table de sonidos (los clips los arrastrás vos en el Inspector) ----
            var audioGO = FindOrCreate("Audio");
            var soundBank = EnsureComponent<SoundBank>(audioGO);
            soundBank.EnsureAllEntries();
            EditorUtility.SetDirty(soundBank);

            var soundPlayer = EnsureComponent<SoundPlayer>(audioGO);
            SetField(soundPlayer, "bank", soundBank);

            var musicGO = FindOrCreate("Music");
            var musicPlayer = EnsureComponent<MusicPlayer>(musicGO);
            SetField(musicPlayer, "bank", soundBank);

            // ---- FX hub: partículas y números de daño ----
            var fxHubGO = FindOrCreate("FxHub");
            var damageNumbers = EnsureComponent<DamageNumberController>(fxHubGO);
            SetField(damageNumbers, "prefab", damageNumberPrefab);

            var deathParticles = EnsureComponent<EnemyDeathParticles>(fxHubGO);
            SetField(deathParticles, "prefab", deathBurstPrefab);

            var impactParticles = EnsureComponent<WeaponImpactParticles>(fxHubGO);
            SetField(impactParticles, "prefab", impactBurstPrefab);
            SetField(impactParticles, "color", new Color(1f, 0.95f, 0.4f));

            // ---- UI: panel de Flyweight, pausa y pantallazo azul ----
            // El orden importa: pausa y pantallazo azul se dibujan ÚLTIMOS (encima de todo).
            BuildFlyweightHud(canvas);
            BuildPauseMenu(canvas, flatSprite, gameManagerGO);
            BuildBsod(canvas);

            // ---- Build Settings: solo MUNDO 1 ----
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log("[Parcial2Setup] Listo: Strategy, Flyweight, Look-Up Table, Decorator y Localization cableados en MUNDO 1.");
        }

        // ---------------- Datos ----------------

        private static void AssignMovementIfEmpty(string enemyDataPath, MovementStrategy movement)
        {
            var enemyData = AssetDatabase.LoadAssetAtPath<EnemyData>(enemyDataPath);
            if (enemyData == null)
            {
                Debug.LogWarning($"[Parcial2Setup] No se encontró {enemyDataPath}.");
                return;
            }

            var serialized = new SerializedObject(enemyData);
            var property = serialized.FindProperty("movement");
            if (property.objectReferenceValue == null)
            {
                property.objectReferenceValue = movement;
                serialized.ApplyModifiedProperties();
                EditorUtility.SetDirty(enemyData);
            }
        }

        private static WeaponData ConfigureExistingWeaponData(string path, string nameKey, SoundId fireSound)
        {
            var data = AssetDatabase.LoadAssetAtPath<WeaponData>(path);
            if (data == null)
            {
                Debug.LogError($"[Parcial2Setup] No se encontró {path}.");
                return null;
            }

            SetField(data, "nameKey", nameKey);
            SetField(data, "fireSound", fireSound);
            BuildTableIfEmpty(data);
            return data;
        }

        private static WeaponData EnsureWeaponData(string path, string weaponName, string nameKey, SoundId fireSound,
            int maxLevel, float baseDamage, float damagePerLevel, float baseCooldown, float cooldownReductionPerLevel,
            float minCooldown, float projectileSpeed, float projectileLifetime)
        {
            var data = LoadOrCreate<WeaponData>(path, created =>
            {
                SetField(created, "weaponName", weaponName);
                SetField(created, "maxLevel", maxLevel);
                SetField(created, "baseDamage", baseDamage);
                SetField(created, "damagePerLevel", damagePerLevel);
                SetField(created, "baseCooldown", baseCooldown);
                SetField(created, "cooldownReductionPerLevel", cooldownReductionPerLevel);
                SetField(created, "minCooldown", minCooldown);
                SetField(created, "projectileSpeed", projectileSpeed);
                SetField(created, "projectileLifetime", projectileLifetime);
            });

            SetField(data, "nameKey", nameKey);
            SetField(data, "fireSound", fireSound);
            BuildTableIfEmpty(data);
            return data;
        }

        // La tabla solo se genera si está vacía: volver a correr el setup no pisa las filas
        // que se hayan retocado a mano en el Inspector.
        private static void BuildTableIfEmpty(WeaponData data)
        {
            var serialized = new SerializedObject(data);
            if (serialized.FindProperty("levelTable").arraySize == 0)
            {
                data.RebuildTable();
                EditorUtility.SetDirty(data);
            }
        }

        // ---------------- Prefabs ----------------

        private static void ConfigureEnemyPrefab(DamageNumberStyle enemyStyle)
        {
            const string path = "Assets/Prefabs/Enemies/Enemy.prefab";
            var contents = PrefabUtility.LoadPrefabContents(path);

            EnsureComponent<HitFlash>(contents);
            EnsureComponent<SquashStretch>(contents);
            var numbers = EnsureComponent<DamageNumberSpawner>(contents);
            SetField(numbers, "style", enemyStyle);

            PrefabUtility.SaveAsPrefabAsset(contents, path);
            PrefabUtility.UnloadPrefabContents(contents);
        }

        private static FloatingDamageNumber BuildDamageNumberPrefab()
        {
            const string path = "Assets/Prefabs/Fx/DamageNumber.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<FloatingDamageNumber>(path);
            if (existing != null)
            {
                return existing;
            }

            var go = new GameObject("DamageNumber");
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var textMesh = go.AddComponent<TextMesh>();
            textMesh.font = font;
            textMesh.fontSize = 48;
            textMesh.characterSize = 0.1f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.color = Color.white;

            var meshRenderer = EnsureComponent<MeshRenderer>(go);
            meshRenderer.sharedMaterial = font.material;
            meshRenderer.sortingOrder = 20;

            go.AddComponent<FloatingDamageNumber>();

            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return savedPrefab.GetComponent<FloatingDamageNumber>();
        }

        private static Material EnsureParticleMaterial()
        {
            const string path = "Assets/Data/Fx/Particle_Sprite.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
            {
                return existing;
            }

            var material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static PooledParticleEffect BuildParticleBurstPrefab(string name, string path, int burstCount,
            float startSpeed, float startSize, float startLifetime, float radius, Material material)
        {
            var existing = AssetDatabase.LoadAssetAtPath<PooledParticleEffect>(path);
            if (existing != null)
            {
                // Prefab generado antes de este arreglo: se le asigna el material si no tiene.
                var contents = PrefabUtility.LoadPrefabContents(path);
                var existingRenderer = contents.GetComponent<ParticleSystemRenderer>();
                if (existingRenderer != null && existingRenderer.sharedMaterial == null)
                {
                    existingRenderer.sharedMaterial = material;
                    PrefabUtility.SaveAsPrefabAsset(contents, path);
                }

                PrefabUtility.UnloadPrefabContents(contents);
                return AssetDatabase.LoadAssetAtPath<PooledParticleEffect>(path);
            }

            var go = new GameObject(name);
            var ps = go.AddComponent<ParticleSystem>();

            var main = ps.main;
            main.loop = false;
            main.playOnAwake = false;
            main.duration = 0.5f;
            main.startLifetime = startLifetime;
            main.startSpeed = startSpeed;
            main.startSize = startSize;
            main.startColor = Color.white;
            main.gravityModifier = 0f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)burstCount) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = radius;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            var gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
            colorOverLifetime.color = gradient;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.2f));

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.sortingOrder = 15;
            renderer.sharedMaterial = material;

            go.AddComponent<PooledParticleEffect>();

            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return savedPrefab.GetComponent<PooledParticleEffect>();
        }

        // ---------------- Visuales de armas ----------------

        private static LineRenderer BuildLineVisual(Transform playerTransform, string name, Color color, float width)
        {
            var existing = playerTransform.Find(name);
            if (existing != null)
            {
                return existing.GetComponent<LineRenderer>();
            }

            var go = new GameObject(name);
            go.transform.SetParent(playerTransform, false);

            var lineRenderer = go.AddComponent<LineRenderer>();
            lineRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.sortingOrder = 10;
            lineRenderer.enabled = false;

            return lineRenderer;
        }

        private static Transform BuildDroneVisual(Transform playerTransform, Sprite circleSprite)
        {
            var existing = playerTransform.Find("DroneVisual");
            if (existing != null)
            {
                EnsureComponent<DroneMovement>(existing.gameObject);
                return existing;
            }

            var go = new GameObject("DroneVisual");
            go.transform.SetParent(playerTransform, false);
            go.transform.localScale = Vector3.one * 0.35f;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = circleSprite;
            sr.color = new Color(0.2f, 1f, 0.6f);

            go.AddComponent<DroneMovement>();
            go.SetActive(false);

            return go.transform;
        }

        private static void SetVisualActive(Transform parent, string childName, bool active)
        {
            var child = parent.Find(childName);
            if (child != null)
            {
                child.gameObject.SetActive(active);
            }
        }

        // ---------------- UI ----------------

        private static void BuildPauseMenu(Transform canvas, Sprite flatSprite, GameObject gameManagerGO)
        {
            var pauseController = EnsureComponent<PauseController>(gameManagerGO);

            var panel = canvas.Find("PauseMenuPanel");
            if (panel == null)
            {
                var panelGO = new GameObject("PauseMenuPanel", typeof(RectTransform));
                panelGO.transform.SetParent(canvas, false);

                var dim = panelGO.AddComponent<Image>();
                dim.color = new Color(0f, 0f, 0f, 0.75f);
                StretchFill(dim.rectTransform);

                var title = CreateText("Title", panelGO.transform, "PAUSA", 56, Color.white, TextAnchor.MiddleCenter);
                SetRect(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    new Vector2(0, 220), new Vector2(800, 90));
                AddLocalizedText(title.transform, LocKeys.PauseTitle);

                CreateMenuButton("Button_Resume", panelGO.transform, "Reanudar", LocKeys.PauseResume, flatSprite, 60f);
                CreateMenuButton("Button_Restart", panelGO.transform, "Reiniciar", LocKeys.PauseRestart, flatSprite, -70f);
                CreateMenuButton("Button_Language", panelGO.transform, "Idioma: Español", LocKeys.PauseLanguage, flatSprite, -200f);

                panelGO.SetActive(false);
                panel = panelGO.transform;
            }

            var menuUI = EnsureComponent<PauseMenuUI>(canvas.gameObject);
            SetField(menuUI, "panelRoot", panel.gameObject);
            SetField(menuUI, "pauseController", pauseController);
            SetField(menuUI, "resumeButton", panel.Find("Button_Resume").GetComponent<Button>());
            SetField(menuUI, "restartButton", panel.Find("Button_Restart").GetComponent<Button>());
            SetField(menuUI, "languageButton", panel.Find("Button_Language").GetComponent<Button>());

            panel.SetAsLastSibling();
        }

        private static void CreateMenuButton(string name, Transform parent, string label, string key, Sprite flatSprite, float y)
        {
            var image = CreateImage(name, parent, new Color(0.12f, 0.12f, 0.12f, 0.95f), flatSprite);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            SetRect(image.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0, y), new Vector2(600, 100));

            var text = CreateText("Label", image.transform, label, 30, Color.white, TextAnchor.MiddleCenter);
            StretchFill(text.rectTransform);
            AddLocalizedText(text.transform, key);
        }

        private static void BuildBsod(Transform canvas)
        {
            var panel = canvas.Find("BsodPanel");
            TextMeshProUGUI messageText;

            if (panel == null)
            {
                var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Fonts/Perfect DOS VGA 437 SDF.asset");

                var panelGO = new GameObject("BsodPanel", typeof(RectTransform));
                panelGO.transform.SetParent(canvas, false);
                StretchFill((RectTransform)panelGO.transform);

                var background = panelGO.AddComponent<Image>();
                background.color = new Color32(0x00, 0x00, 0xAA, 0xFF); // el azul clásico del pantallazo

                var messageGO = new GameObject("Message", typeof(RectTransform));
                messageGO.transform.SetParent(panelGO.transform, false);
                SetRect((RectTransform)messageGO.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1100, 600));

                messageText = messageGO.AddComponent<TextMeshProUGUI>();
                messageText.font = font;
                messageText.fontSize = 32;
                messageText.color = Color.white;
                messageText.alignment = TextAlignmentOptions.TopLeft;
                messageText.text = string.Empty;

                panelGO.SetActive(false);
                panel = panelGO.transform;
            }
            else
            {
                messageText = panel.Find("Message").GetComponent<TextMeshProUGUI>();
            }

            var bsodUI = EnsureComponent<BsodUI>(canvas.gameObject);
            SetField(bsodUI, "panelRoot", panel.gameObject);
            SetField(bsodUI, "messageText", messageText);

            panel.SetAsLastSibling();
        }

        private static void BuildFlyweightHud(Transform canvas)
        {
            var root = canvas.Find("FlyweightStats");
            if (root == null)
            {
                var rootGO = new GameObject("FlyweightStats", typeof(RectTransform));
                rootGO.transform.SetParent(canvas, false);
                SetRect((RectTransform)rootGO.transform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(20, 20), new Vector2(1100, 130));

                var text = CreateText("Text", rootGO.transform, string.Empty, 20, new Color(0.7f, 1f, 0.7f), TextAnchor.LowerLeft);
                StretchFill(text.rectTransform);
                text.gameObject.AddComponent<Shadow>();

                root = rootGO.transform;
            }

            var statsUI = EnsureComponent<FlyweightStatsUI>(root.gameObject);
            var textChild = root.Find("Text");
            SetField(statsUI, "statsText", textChild.GetComponent<Text>());
            SetField(statsUI, "visualRoot", textChild.gameObject);
        }

        private static void AddLocalizedText(Transform textTransform, string key)
        {
            if (textTransform == null)
            {
                Debug.LogWarning($"[Parcial2Setup] No se encontró el texto para la clave '{key}'.");
                return;
            }

            var localized = EnsureComponent<LocalizedText>(textTransform.gameObject);
            SetField(localized, "key", key);
        }

        // ---------------- Debug (solo Editor, solo en Play) ----------------

        [MenuItem("Tools/Arena Survivor/Parcial 2/Debug/Alternar pausa (Play)")]
        private static void DebugTogglePause()
        {
            var pause = Object.FindFirstObjectByType<PauseController>();
            if (pause != null)
            {
                pause.TogglePause();
            }
        }

        [MenuItem("Tools/Arena Survivor/Parcial 2/Debug/Subir de nivel (Play)")]
        private static void DebugLevelUp()
        {
            EventManager.Publish(new PlayerLeveledUpEvent(99));
        }

        [MenuItem("Tools/Arena Survivor/Parcial 2/Debug/Desbloquear todas las armas (Play)")]
        private static void DebugUnlockWeapons()
        {
            foreach (var weapon in Object.FindObjectsByType<WeaponBase>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                weapon.enabled = true;
            }
        }

        [MenuItem("Tools/Arena Survivor/Parcial 2/Debug/Matar al jugador (Play)")]
        private static void DebugKillPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && player.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(999999f);
            }
        }

        [MenuItem("Tools/Arena Survivor/Parcial 2/Debug/Resetear idioma guardado")]
        private static void DebugResetLanguage()
        {
            PlayerPrefs.DeleteKey("ArenaSurvivor.Language");
            PlayerPrefs.Save();
            Debug.Log("[Parcial2Setup] Idioma guardado borrado: la próxima partida arranca en español.");
        }

        // Validadores: los items de Debug solo se habilitan mientras el juego está en Play.
        [MenuItem("Tools/Arena Survivor/Parcial 2/Debug/Alternar pausa (Play)", true)]
        private static bool ValidateTogglePause() => EditorApplication.isPlaying;

        [MenuItem("Tools/Arena Survivor/Parcial 2/Debug/Subir de nivel (Play)", true)]
        private static bool ValidateLevelUp() => EditorApplication.isPlaying;

        [MenuItem("Tools/Arena Survivor/Parcial 2/Debug/Desbloquear todas las armas (Play)", true)]
        private static bool ValidateUnlockWeapons() => EditorApplication.isPlaying;

        [MenuItem("Tools/Arena Survivor/Parcial 2/Debug/Matar al jugador (Play)", true)]
        private static bool ValidateKillPlayer() => EditorApplication.isPlaying;

        // ---------------- Utilidades ----------------

        private static T LoadOrCreate<T>(string path, Action<T> initialize = null) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
            {
                return existing;
            }

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            initialize?.Invoke(asset);
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static GameObject FindOrCreate(string name)
        {
            var existing = GameObject.Find(name);
            return existing != null ? existing : new GameObject(name);
        }

        private static T EnsureComponent<T>(GameObject go) where T : Component
        {
            return EnsureComponent<T>(go, out _);
        }

        private static T EnsureComponent<T>(GameObject go, out bool created) where T : Component
        {
            var existing = go.GetComponent<T>();
            created = existing == null;
            return created ? go.AddComponent<T>() : existing;
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

        private static Text CreateText(string name, Transform parent, string content, int fontSize, Color color, TextAnchor alignment)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            return text;
        }

        private static void SetField(Object target, string fieldName, object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop == null)
            {
                Debug.LogError($"[Parcial2Setup] Campo '{fieldName}' no encontrado en {target.GetType().Name}.");
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
                case Enum e:
                    prop.enumValueIndex = Convert.ToInt32(e);
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
                    Debug.LogError($"[Parcial2Setup] Tipo no soportado para '{fieldName}': {value.GetType()}");
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
    }
}
