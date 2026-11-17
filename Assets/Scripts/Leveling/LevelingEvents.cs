using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Leveling
{
    /// <summary>
    /// Se publica cuando LevelUpManager efectivamente abre el panel de elegir mejora.
    /// Todo lo que reacciona a "el panel está abierto" (sonido, música) escucha esto en vez
    /// de conocer al panel.
    /// </summary>
    public readonly struct ChoicePanelOpenedEvent : IGameEvent
    {
    }

    /// <summary>
    /// Se publica cuando el jugador elige una mejora en el panel (el panel se cierra y el
    /// juego reanuda). Sirve para el sonido de confirmación.
    /// </summary>
    public readonly struct UpgradeChosenEvent : IGameEvent
    {
    }
}
