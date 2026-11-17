using UnityEngine;

namespace ArenaSurvivor.Enemies.Movement
{
    /// <summary>
    /// Base ScriptableObject de las estrategias de movimiento: cada forma de moverse es un
    /// asset que se arrastra al EnemyData desde el Inspector (Unity no puede serializar una
    /// interfaz directamente, por eso la estrategia concreta hereda de acá).
    /// Las estrategias no guardan estado por enemigo — reciben todo en el MovementContext —
    /// así que un mismo asset lo comparten todos los enemigos que lo usan.
    /// </summary>
    public abstract class MovementStrategy : ScriptableObject, IMovementStrategy
    {
        public abstract Vector2 CalculateVelocity(MovementContext context);
    }
}
