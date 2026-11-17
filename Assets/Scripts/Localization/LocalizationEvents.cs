using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Localization
{
    /// <summary>
    /// Se publica cada vez que cambia el idioma (y una vez al arrancar la escena). Todo
    /// texto que se vea en pantalla escucha esto y se vuelve a dibujar en el idioma nuevo —
    /// el manager no conoce a ninguno de ellos (Observer).
    /// </summary>
    public readonly struct LanguageChangedEvent : IGameEvent
    {
        public readonly Language Language;

        public LanguageChangedEvent(Language language)
        {
            Language = language;
        }
    }
}
