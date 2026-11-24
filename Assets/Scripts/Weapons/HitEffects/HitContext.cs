using UnityEngine;

namespace ArenaSurvivor.Weapons.HitEffects
{
    // Los datos de UN golpe, armados por quien lo da (proyectil, láser, orbe...) y pasados a la cadena de efectos
    public readonly struct HitContext
    {
        public readonly Component Target;
        public readonly Vector2 Point;
        public readonly Vector2 Direction;
        public readonly float Damage;

        public HitContext(Component target, Vector2 point, Vector2 direction, float damage)
        {
            Target = target;
            Point = point;
            Direction = direction;
            Damage = damage;
        }
    }
}
