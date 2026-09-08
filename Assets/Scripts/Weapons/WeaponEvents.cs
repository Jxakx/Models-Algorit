using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Weapons
{
    /// <summary>
    /// Se publica cada vez que un arma del jugador cambia de estado relevante para el HUD:
    /// se desbloquea por primera vez (Start corre recién cuando el componente se habilita) o
    /// sube de nivel. Cada arma se reporta a sí misma — el HUD no conoce a WeaponBase ni al
    /// Player, solo arma su lista escuchando este evento (Observer).
    /// </summary>
    public readonly struct WeaponChangedEvent : IGameEvent
    {
        public readonly string WeaponName;
        public readonly int Level;
        public readonly int MaxLevel;

        public WeaponChangedEvent(string weaponName, int level, int maxLevel)
        {
            WeaponName = weaponName;
            Level = level;
            MaxLevel = maxLevel;
        }
    }
}
