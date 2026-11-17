using ArenaSurvivor.Core.Combat;

namespace ArenaSurvivor.Weapons.HitEffects
{
    /// <summary>Decorador: además del golpe, empuja al objetivo en la dirección del impacto.</summary>
    public sealed class KnockbackDecorator : HitEffectDecorator
    {
        private readonly float _force;

        public KnockbackDecorator(IHitEffect inner, float force) : base(inner)
        {
            _force = force;
        }

        protected override void ApplyExtra(HitContext context)
        {
            if (context.Target != null && context.Target.TryGetComponent<IKnockbackable>(out var knockbackable))
            {
                knockbackable.ApplyKnockback(context.Direction, _force);
            }
        }
    }
}
