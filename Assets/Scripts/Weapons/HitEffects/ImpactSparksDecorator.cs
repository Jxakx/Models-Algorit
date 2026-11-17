using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Fx;

namespace ArenaSurvivor.Weapons.HitEffects
{
    /// <summary>
    /// Decorador: además del golpe, avisa que hubo un impacto en ese punto. No dibuja nada
    /// él mismo — publica WeaponImpactEvent y quien escucha (chispas, sonido) decide qué
    /// hacer (Observer), así el arma no conoce ni partículas ni audio.
    /// </summary>
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
