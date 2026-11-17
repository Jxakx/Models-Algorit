using UnityEngine;

namespace ArenaSurvivor.Weapons.HitEffects
{
    /// <summary>
    /// Los datos de UN golpe, armados por quien lo da (proyectil, láser, orbe...) y pasados
    /// a la cadena de efectos. El objetivo se guarda como Component genérico, no como Enemy:
    /// cada efecto le pide la capacidad que necesita (IDamageable, IKnockbackable,
    /// ISlowable) y se salta el golpe si el objetivo no la tiene.
    /// </summary>
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
