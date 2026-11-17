using UnityEngine;

namespace ArenaSurvivor.Enemies.Movement
{
    /// <summary>Perseguir en línea recta al objetivo a velocidad constante (el comportamiento original).</summary>
    [CreateAssetMenu(fileName = "Movement_Chase", menuName = "Arena Survivor/Movement/Chase")]
    public class ChaseMovement : MovementStrategy
    {
        public override Vector2 CalculateVelocity(MovementContext context)
        {
            return context.DirectionToTarget * context.Speed;
        }
    }
}
