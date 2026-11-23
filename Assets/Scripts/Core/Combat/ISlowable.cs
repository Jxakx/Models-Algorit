namespace ArenaSurvivor.Core.Combat
{
    // Algo que puede ser ralentizado un rato
    public interface ISlowable
    {
        /// <param name="speedMultiplier">1 = velocidad normal, 0.5 = a la mitad.</param>
        /// <param name="duration">Segundos que dura la ralentización.</param>
        void ApplySlow(float speedMultiplier, float duration);
    }
}
