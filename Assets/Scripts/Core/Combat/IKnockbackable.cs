using UnityEngine;

namespace ArenaSurvivor.Core.Combat
{
    //Algo que puede recibir un empujón físico al ser golpeado
    public interface IKnockbackable
    {
        void ApplyKnockback(Vector2 direction, float force);
    }
}
