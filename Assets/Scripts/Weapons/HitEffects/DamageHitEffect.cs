using ArenaSurvivor.Core.Combat;

namespace ArenaSurvivor.Weapons.HitEffects
{
    public sealed class DamageHitEffect : IHitEffect
    {
        public void Apply(HitContext context)
        {
            if (context.Target != null && context.Target.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(context.Damage);
            }
        }
    }
}
