using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Managers
{
    public readonly struct GamePausedEvent : IGameEvent
    {
    }

    public readonly struct GameResumedEvent : IGameEvent
    {
    }
}
