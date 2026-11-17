using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.UI
{
    /// <summary>Se publica cuando el jugador aprieta un botón del menú. Solo avisa — el sonido lo decide quien escucha.</summary>
    public readonly struct UiButtonClickedEvent : IGameEvent
    {
    }
}
