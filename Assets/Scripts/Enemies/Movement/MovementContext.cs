using UnityEngine;

namespace ArenaSurvivor.Enemies.Movement
{
    // Todo lo que una estrategia de movimiento necesita saber para decidir hacia dónde va un enemigo en este instante
    public readonly struct MovementContext
    {
        public readonly Vector2 Position;
        public readonly Vector2 TargetPosition;

        //Velocidad base del tipo de enemigo, ya con la ralentización aplicada
        public readonly float Speed;

        public readonly float Time;

        //Número fijo entre 0 y 1 por enemigo: desfasa a cada uno para que no se muevan todos sincronizados
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
