using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Localization
{
    // Se publica cada vez que cambia el idioma 
    public readonly struct LanguageChangedEvent : IGameEvent
    {
        public readonly Language Language;

        public LanguageChangedEvent(Language language)
        {
            Language = language;
        }
    }
}
