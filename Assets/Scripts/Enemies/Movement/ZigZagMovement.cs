using UnityEngine;

namespace ArenaSurvivor.Enemies.Movement
{
    /// <summary>
    /// Avanza hacia el objetivo pero serpenteando de lado a lado: más difícil de apuntar y
    /// de esquivar en masa. Cada enemigo arranca desfasado (Seed), así el enjambre no
    /// zigzaguea todo al mismo tiempo.
    /// </summary>
    [CreateAssetMenu(fileName = "Movement_ZigZag", menuName = "Arena Survivor/Movement/ZigZag")]
    public class ZigZagMovement : MovementStrategy
    {
        [Tooltip("Cuántas oscilaciones por segundo.")]
        [SerializeField] private float frequency = 3f;

        [Tooltip("Qué tanto se desvía hacia los costados (0 = nada, 1 = tanto como avanza).")]
        [SerializeField] private float lateralAmount = 0.9f;

        public override Vector2 CalculateVelocity(MovementContext context)
        {
            var forward = context.DirectionToTarget;
            var side = new Vector2(-forward.y, forward.x);
            var wobble = Mathf.Sin(context.Time * frequency + context.Seed * Mathf.PI * 2f) * lateralAmount;

            return (forward + side * wobble).normalized * context.Speed;
        }
    }
}
