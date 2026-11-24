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
    // Panel de demostración del patrón Flyweight
    public class FlyweightStatsUI : MonoBehaviour
    {
        private const float RefreshInterval = 0.25f;

        [SerializeField] private Text statsText;
        [SerializeField] private GameObject visualRoot;

        private float _nextRefreshTime;

        private void Awake()
        {
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
