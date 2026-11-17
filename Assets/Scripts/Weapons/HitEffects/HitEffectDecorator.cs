namespace ArenaSurvivor.Weapons.HitEffects
{
    /// <summary>
    /// Decorador base: ES un IHitEffect y además TIENE un IHitEffect adentro (el que envuelve).
    /// Al aplicarse, primero hace lo suyo (ApplyExtra) y después delega en el efecto envuelto.
    /// Los decoradores concretos solo escriben ApplyExtra.
    ///
    /// El daño (DamageHitEffect) es siempre el efecto más interno, o sea el ÚLTIMO en
    /// ejecutarse: así el empujón y la ralentización se aplican sobre un enemigo todavía
    /// vivo, antes de que el daño lo mate y el pool lo recicle.
    /// </summary>
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
