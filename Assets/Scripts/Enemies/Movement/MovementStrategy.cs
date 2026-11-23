using UnityEngine;

namespace ArenaSurvivor.Enemies.Movement
{
    public abstract class MovementStrategy : ScriptableObject, IMovementStrategy
    {
        public abstract Vector2 CalculateVelocity(MovementContext context);
    }
}
