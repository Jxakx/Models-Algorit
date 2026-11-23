using UnityEngine;

namespace ArenaSurvivor.Enemies.Movement
{
    // Avanza despacio y cada tanto pega una embestida corta mucho más rápida
    [CreateAssetMenu(fileName = "Movement_Charge", menuName = "Arena Survivor/Movement/Charge")]
    public class ChargeMovement : MovementStrategy
    {
        [Tooltip("Cada cuántos segundos se repite el ciclo (caminar + embestir).")]
        [SerializeField] private float cycleDuration = 3.5f;

        [Tooltip("Cuántos segundos dura la embestida dentro de cada ciclo.")]
        [SerializeField] private float chargeDuration = 0.6f;

        [SerializeField] private float chargeSpeedMultiplier = 3.5f;
        [SerializeField] private float walkSpeedMultiplier = 0.7f;

        public override Vector2 CalculateVelocity(MovementContext context)
        {
            var phase = Mathf.Repeat(context.Time + context.Seed * cycleDuration, cycleDuration);
            var multiplier = phase < chargeDuration ? chargeSpeedMultiplier : walkSpeedMultiplier;

            return context.DirectionToTarget * (context.Speed * multiplier);
        }
    }
}
