namespace ArenaSurvivor.Weapons.HitEffects
{
    /// <summary>
    /// Componente del patrón Decorator: "lo que pasa cuando un arma conecta un golpe".
    /// El efecto base (DamageHitEffect) y todos los decoradores comparten esta misma
    /// interfaz, así quien da el golpe (Projectile, OrbitalHitbox, LaserWeapon...) no sabe
    /// ni le importa cuántos efectos hay apilados detrás.
    /// </summary>
    public interface IHitEffect
    {
        void Apply(HitContext context);
    }
}
