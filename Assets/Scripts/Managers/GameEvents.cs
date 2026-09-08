using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Managers
{
    /// <summary>Se publica cada frame con el tiempo total sobrevivido (para el timer del HUD).</summary>
    public readonly struct GameTimeChangedEvent : IGameEvent
    {
        public readonly float ElapsedSeconds;

        public GameTimeChangedEvent(float elapsedSeconds)
        {
            ElapsedSeconds = elapsedSeconds;
        }
    }

    /// <summary>Se publica cada vez que cambia la cantidad de enemigos eliminados.</summary>
    public readonly struct KillCountChangedEvent : IGameEvent
    {
        public readonly int Count;

        public KillCountChangedEvent(int count)
        {
            Count = count;
        }
    }

    /// <summary>Se publica una vez cuando el jugador pierde.</summary>
    public readonly struct GameOverEvent : IGameEvent
    {
    }

    /// <summary>Se publica una vez cuando el jugador sobrevive el tiempo objetivo.</summary>
    public readonly struct GameWonEvent : IGameEvent
    {
    }
}
