namespace ArenaSurvivor.Core.Combat
{
    /// <summary>
    /// Contrato para cualquier cosa que pueda recibir daño. Quien inflige daño (un proyectil,
    /// un enemigo por contacto) no necesita saber si golpeó al Player o a un enemigo: solo
    /// necesita un IDamageable (Dependency Inversion).
    /// </summary>
    public interface IDamageable
    {
        bool IsAlive { get; }
        void TakeDamage(float amount);
    }
}
