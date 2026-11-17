namespace ArenaSurvivor.Localization
{
    /// <summary>
    /// Lo que el resto del juego necesita del sistema de idiomas: pedir un texto por clave
    /// y cambiar de idioma. La UI y los managers dependen de esta interfaz, no del
    /// LocalizationManager concreto (Dependency Inversion).
    /// </summary>
    public interface ILocalizationService
    {
        Language CurrentLanguage { get; }

        /// <summary>El texto de esa clave en el idioma actual (si no existe, devuelve la clave).</summary>
        string Get(string key);

        /// <summary>Igual que Get(key), pero rellenando los {0}, {1}... del texto con los argumentos.</summary>
        string Get(string key, params object[] args);

        void SetLanguage(Language language);

        void ToggleLanguage();
    }
}
