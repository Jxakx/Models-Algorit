using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Leveling
{
    //Se publica cuando LevelUpManager efectivamente abre el panel de elegir mejora.
    public readonly struct ChoicePanelOpenedEvent : IGameEvent
    {
    }

    // Se publica cuando el jugador elige una mejora en el panel
    public readonly struct UpgradeChosenEvent : IGameEvent
    {
    }
}
