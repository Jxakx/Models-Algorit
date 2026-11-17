using ArenaSurvivor.Core.Combat;

namespace ArenaSurvivor.Weapons.HitEffects
{
    /// <summary>Decorador: además del golpe, ralentiza al objetivo durante un rato.</summary>
    public sealed class SlowDecorator : HitEffectDecorator
    {
        private readonly float _speedMultiplier;
        private readonly float _duration;

        public SlowDecorator(IHitEffect inner, float speedMultiplier, float duration) : base(inner)
        {
            _speedMultiplier = speedMultiplier;
            _duration = duration;
        }

        protected override void ApplyExtra(HitContext context)
        {
            if (context.Target != null && context.Target.TryGetComponent<ISlowable>(out var slowable))
            {
                slowable.ApplySlow(_speedMultiplier, _duration);
            }
        }
    }
}
