namespace ArenaSurvivor.Localization
{
    public interface ILocalizationService
    {
        Language CurrentLanguage { get; }

        string Get(string key);

        string Get(string key, params object[] args);

        void SetLanguage(Language language);

        void ToggleLanguage();
    }
}
