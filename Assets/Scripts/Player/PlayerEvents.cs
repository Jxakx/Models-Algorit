using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Player
{
    /// <summary>Se publica cada vez que cambia la vida del jugador (daño o cura).</summary>
    public readonly struct PlayerHealthChangedEvent : IGameEvent
    {
        public readonly float CurrentHealth;
        public readonly float MaxHealth;

        public PlayerHealthChangedEvent(float currentHealth, float maxHealth)
        {
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
        }
    }

    /// <summary>Se publica una sola vez, cuando la vida del jugador llega a 0.</summary>
    public readonly struct PlayerDiedEvent : IGameEvent
    {
    }

    /// <summary>Se publica cuando una gema de XP es recolectada.</summary>
    public readonly struct XpCollectedEvent : IGameEvent
    {
        public readonly int Amount;

        public XpCollectedEvent(int amount)
        {
            Amount = amount;
        }
    }

    /// <summary>Se publica cada vez que cambia el progreso de XP del jugador (para la barra de XP).</summary>
    public readonly struct PlayerXpChangedEvent : IGameEvent
    {
        public readonly int CurrentXp;
        public readonly int XpToNextLevel;
        public readonly int Level;

        public PlayerXpChangedEvent(int currentXp, int xpToNextLevel, int level)
        {
            CurrentXp = currentXp;
            XpToNextLevel = xpToNextLevel;
            Level = level;
        }
    }

    /// <summary>Se publica cuando el jugador sube de nivel (dispara el panel de mejoras).</summary>
    public readonly struct PlayerLeveledUpEvent : IGameEvent
    {
        public readonly int NewLevel;

        public PlayerLeveledUpEvent(int newLevel)
        {
            NewLevel = newLevel;
        }
    }

    /// <summary>Se publica cada vez que cambia algún multiplicador de PlayerStats (para el HUD).</summary>
    public readonly struct PlayerStatsChangedEvent : IGameEvent
    {
        public readonly float MoveSpeedMultiplier;
        public readonly float DamageMultiplier;

        public PlayerStatsChangedEvent(float moveSpeedMultiplier, float damageMultiplier)
        {
            MoveSpeedMultiplier = moveSpeedMultiplier;
            DamageMultiplier = damageMultiplier;
        }
    }
}
