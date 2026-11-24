using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Weapons
{
    // Se publica cada vez que un arma del jugador cambia de estado relevante para el HUD
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
