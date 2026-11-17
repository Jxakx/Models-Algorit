namespace ArenaSurvivor.Core.Combat
{
    /// <summary>
    /// Algo que puede ser ralentizado un rato. Interfaz propia (ISP), igual que
    /// IKnockbackable: recibir daño, un empujón o una ralentización son capacidades
    /// distintas, y quien aplica el efecto (un decorador de golpe) solo pide la que usa.
    /// </summary>
    public interface ISlowable
    {
        /// <param name="speedMultiplier">1 = velocidad normal, 0.5 = a la mitad.</param>
        /// <param name="duration">Segundos que dura la ralentización.</param>
        void ApplySlow(float speedMultiplier, float duration);
    }
}
