using UnityEngine;

namespace ArenaSurvivor.Core.Combat
{
    /// <summary>
    /// Algo que puede recibir un empujón físico al ser golpeado. Interfaz separada de
    /// IDamageable (ISP): recibir daño y recibir un empujón son cosas distintas — un objeto
    /// podría querer una sin la otra (por eso no se agregó como método más a IDamageable).
    /// </summary>
    public interface IKnockbackable
    {
        void ApplyKnockback(Vector2 direction, float force);
    }
}
