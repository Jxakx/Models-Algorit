using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Player;

namespace ArenaSurvivor.UI
{
    /// <summary>Muestra los multiplicadores de velocidad y daño como porcentaje. Solo escucha PlayerStatsChangedEvent.</summary>
    public class PlayerStatsUI : MonoBehaviour
    {
        [SerializeField] private Text statsText;

        private void OnEnable()
        {
            EventManager.Subscribe<PlayerStatsChangedEvent>(OnStatsChanged);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<PlayerStatsChangedEvent>(OnStatsChanged);
        }

        private void OnStatsChanged(PlayerStatsChangedEvent gameEvent)
        {
            var speedPercent = Mathf.RoundToInt(gameEvent.MoveSpeedMultiplier * 100f);
            var damagePercent = Mathf.RoundToInt(gameEvent.DamageMultiplier * 100f);
            statsText.text = $"Velocidad: {speedPercent}%   Daño: {damagePercent}%";
        }
    }
}
