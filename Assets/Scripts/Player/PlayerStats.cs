using UnityEngine;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Player
{
    /// <summary>
    /// Multiplicadores acumulables que las mejoras de nivel aplican sobre el jugador.
    /// PlayerController y las armas lo leen; el LevelUpManager lo escribe. Publica un evento
    /// cada vez que cambia para que el HUD pueda mostrar el % actual (Observer).
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        public float MoveSpeedMultiplier { get; private set; } = 1f;
        public float DamageMultiplier { get; private set; } = 1f;

        private void OnEnable()
        {
            PublishChanged();
        }

        public void AddMoveSpeedPercent(float percent)
        {
            MoveSpeedMultiplier += percent;
            PublishChanged();
        }

        public void AddDamagePercent(float percent)
        {
            DamageMultiplier += percent;
            PublishChanged();
        }

        private void PublishChanged()
        {
            EventManager.Publish(new PlayerStatsChangedEvent(MoveSpeedMultiplier, DamageMultiplier));
        }
    }
}
