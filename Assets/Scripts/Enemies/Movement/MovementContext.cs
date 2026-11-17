using UnityEngine;

namespace ArenaSurvivor.Enemies.Movement
{
    /// <summary>
    /// Todo lo que una estrategia de movimiento necesita saber para decidir hacia dónde
    /// va un enemigo en este instante. Es el estado EXTRÍNSECO (propio de cada enemigo y de
    /// cada momento) que el Enemy le pasa a la estrategia: así la estrategia en sí queda sin
    /// estado y UN solo asset puede mover a los 150 enemigos de un tipo (Flyweight).
    /// </summary>
    public readonly struct MovementContext
    {
        public readonly Vector2 Position;
        public readonly Vector2 TargetPosition;

        /// <summary>Velocidad base del tipo de enemigo, ya con la ralentización aplicada.</summary>
        public readonly float Speed;

        public readonly float Time;

        /// <summary>Número fijo entre 0 y 1 por enemigo: desfasa a cada uno para que no se muevan todos sincronizados.</summary>
        public readonly float Seed;

        public MovementContext(Vector2 position, Vector2 targetPosition, float speed, float time, float seed)
        {
            Position = position;
            TargetPosition = targetPosition;
            Speed = speed;
            Time = time;
            Seed = seed;
        }

        public Vector2 DirectionToTarget => (TargetPosition - Position).normalized;
    }
}
