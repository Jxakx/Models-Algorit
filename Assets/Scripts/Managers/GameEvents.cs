using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Managers
{
    //Se publica cada frame con el tiempo total sobrevivido
    public readonly struct GameTimeChangedEvent : IGameEvent
    {
        public readonly float ElapsedSeconds;

        public GameTimeChangedEvent(float elapsedSeconds)
        {
            ElapsedSeconds = elapsedSeconds;
        }
    }

    //Se publica cada vez que cambia la cantidad de enemigos eliminados
    public readonly struct KillCountChangedEvent : IGameEvent
    {
        public readonly int Count;

        public KillCountChangedEvent(int count)
        {
            Count = count;
        }
    }

    //Se publica una vez cuando el jugador pierde
    public readonly struct GameOverEvent : IGameEvent
    {
    }

    //Se publica una vez cuando el jugador sobrevive el tiempo objetivo
    public readonly struct GameWonEvent : IGameEvent
    {
    }
}
