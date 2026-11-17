using ArenaSurvivor.Core.Combat;

namespace ArenaSurvivor.Weapons.HitEffects
{
    /// <summary>
    /// Componente concreto del Decorator: el efecto de golpe más simple, solo hace daño.
    /// Todo lo demás (empujón, ralentización, chispas) se le va envolviendo encima con
    /// decoradores — cada arma arma su propia combinación sin heredar de nada.
    /// </summary>
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
