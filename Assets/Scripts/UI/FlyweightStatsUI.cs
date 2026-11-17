using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Core.Flyweight;
using ArenaSurvivor.Enemies;
using ArenaSurvivor.Fx;
using ArenaSurvivor.Localization;
using ArenaSurvivor.Weapons;

namespace ArenaSurvivor.UI
{
    /// <summary>
    /// Panel de demostración del patrón Flyweight: muestra, en vivo, cuántos objetos están
    /// usando un dato compartido y cuántos datos compartidos hay en memoria. Por ejemplo,
    /// "87 enemigos vivos -> 3 EnemyData": los 87 no tienen cada uno su copia de las
    /// estadísticas, todos apuntan a las mismas 3. F3 lo muestra u oculta.
    /// </summary>
    public class FlyweightStatsUI : MonoBehaviour
    {
        private const float RefreshInterval = 0.25f;

        [SerializeField] private Text statsText;
        [SerializeField] private GameObject visualRoot;

        private float _nextRefreshTime;

        private void Awake()
        {
            // Arranca de cero los contadores de uso: los objetos de la escena anterior (si la
            // hubo) se destruyeron sin avisar, y sin esto el conteo los arrastraría.
            FlyweightFactory<EnemyData>.ResetUsage();
            FlyweightFactory<WeaponData>.ResetUsage();
            FlyweightFactory<DamageNumberStyle>.ResetUsage();
        }

        private void OnEnable()
        {
            EventManager.Subscribe<LanguageChangedEvent>(OnLanguageChanged);
            Redraw();
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<LanguageChangedEvent>(OnLanguageChanged);
        }

        private void Update()
        {
            // El script vive en un objeto que siempre está activo; lo que se prende y se
            // apaga es el hijo con el texto (si el script viviera en el mismo objeto que
            // apaga, nadie volvería a escuchar F3 para prenderlo de nuevo).
            if (Input.GetKeyDown(KeyCode.F3) && visualRoot != null)
            {
                visualRoot.SetActive(!visualRoot.activeSelf);
            }

            // Tiempo real, no el del juego: el panel sigue actualizándose aunque haya pausa.
            if (Time.unscaledTime >= _nextRefreshTime)
            {
                _nextRefreshTime = Time.unscaledTime + RefreshInterval;
                Redraw();
            }
        }

        private void OnLanguageChanged(LanguageChangedEvent gameEvent) => Redraw();

        private void Redraw()
        {
            statsText.text =
                Localizer.Get(LocKeys.FlyTitle) + "\n" +
                Localizer.Get(LocKeys.FlyEnemies,
                    FlyweightFactory<EnemyData>.ContextCount, FlyweightFactory<EnemyData>.SharedCount) + "\n" +
                Localizer.Get(LocKeys.FlyProjectiles,
                    FlyweightFactory<WeaponData>.ContextCount, FlyweightFactory<WeaponData>.SharedCount) + "\n" +
                Localizer.Get(LocKeys.FlyNumbers,
                    FlyweightFactory<DamageNumberStyle>.ContextCount, FlyweightFactory<DamageNumberStyle>.SharedCount);
        }
    }
}
