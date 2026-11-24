namespace ArenaSurvivor.Weapons.HitEffects
{
    public abstract class HitEffectDecorator : IHitEffect
    {
        private readonly IHitEffect _inner;

        protected HitEffectDecorator(IHitEffect inner)
        {
            _inner = inner;
        }

        public void Apply(HitContext context)
        {
            ApplyExtra(context);
            _inner.Apply(context);
        }

        protected abstract void ApplyExtra(HitContext context);
    }
}
