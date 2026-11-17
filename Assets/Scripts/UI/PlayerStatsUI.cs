using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Localization;
using ArenaSurvivor.Player;

namespace ArenaSurvivor.UI
{
    /// <summary>
    /// Muestra los multiplicadores de velocidad y daño como porcentaje. Escucha
    /// PlayerStatsChangedEvent y LanguageChangedEvent (para traducir las etiquetas).
    /// </summary>
    public class PlayerStatsUI : MonoBehaviour
    {
        [SerializeField] private Text statsText;

        private float _moveSpeedMultiplier = 1f;
        private float _damageMultiplier = 1f;

        private void OnEnable()
        {
            EventManager.Subscribe<PlayerStatsChangedEvent>(OnStatsChanged);
            EventManager.Subscribe<LanguageChangedEvent>(OnLanguageChanged);
            Redraw();
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<PlayerStatsChangedEvent>(OnStatsChanged);
            EventManager.Unsubscribe<LanguageChangedEvent>(OnLanguageChanged);
        }

        private void OnStatsChanged(PlayerStatsChangedEvent gameEvent)
        {
            _moveSpeedMultiplier = gameEvent.MoveSpeedMultiplier;
            _damageMultiplier = gameEvent.DamageMultiplier;
            Redraw();
        }

        private void OnLanguageChanged(LanguageChangedEvent gameEvent) => Redraw();

        private void Redraw()
        {
            var speedPercent = Mathf.RoundToInt(_moveSpeedMultiplier * 100f);
            var damagePercent = Mathf.RoundToInt(_damageMultiplier * 100f);
            statsText.text = Localizer.Get(LocKeys.HudStats, speedPercent, damagePercent);
        }
    }
}
