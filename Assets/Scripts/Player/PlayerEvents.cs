using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Player
{
    //Se publica cada vez que cambia la vida del jugador
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

    //Se publica cuando la vida del jugador llega a 0
    public readonly struct PlayerDiedEvent : IGameEvent
    {
    }

    //Se publica cuando una gema de XP es recolectada
    public readonly struct XpCollectedEvent : IGameEvent
    {
        public readonly int Amount;

        public XpCollectedEvent(int amount)
        {
            Amount = amount;
        }
    }

    //Se publica cada vez que cambia el progreso de XP del jugador
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

    //Se publica cuando el jugador sube de nivel
    public readonly struct PlayerLeveledUpEvent : IGameEvent
    {
        public readonly int NewLevel;

        public PlayerLeveledUpEvent(int newLevel)
        {
            NewLevel = newLevel;
        }
    }

    //Se publica cada vez que cambia algún multiplicador de PlayerStats
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
