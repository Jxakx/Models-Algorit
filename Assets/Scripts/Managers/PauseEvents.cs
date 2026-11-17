using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Managers
{
    /// <summary>Se publica cuando el jugador pausa la partida (ESC). El menú de pausa se muestra al recibirlo.</summary>
    public readonly struct GamePausedEvent : IGameEvent
    {
    }

    /// <summary>Se publica cuando la partida se reanuda. El menú de pausa se oculta al recibirlo.</summary>
    public readonly struct GameResumedEvent : IGameEvent
    {
    }
}
