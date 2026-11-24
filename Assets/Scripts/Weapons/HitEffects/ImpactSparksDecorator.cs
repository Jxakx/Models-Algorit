using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Fx;

namespace ArenaSurvivor.Weapons.HitEffects
{
    public sealed class ImpactSparksDecorator : HitEffectDecorator
    {
        public ImpactSparksDecorator(IHitEffect inner) : base(inner)
        {
        }

        protected override void ApplyExtra(HitContext context)
        {
            EventManager.Publish(new WeaponImpactEvent(context.Point));
        }
    }
}
