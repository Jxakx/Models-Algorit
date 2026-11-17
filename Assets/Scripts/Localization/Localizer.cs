namespace ArenaSurvivor.Localization
{
    /// <summary>
    /// Punto de acceso corto al servicio de idiomas de la escena. Si todavía no hay un
    /// LocalizationManager activo, devuelve la clave tal cual en vez de romperse: el juego
    /// sigue funcionando y el texto faltante se nota en pantalla.
    /// </summary>
    public static class Localizer
    {
        public static string Get(string key)
        {
            var service = LocalizationManager.Service;
            return service != null ? service.Get(key) : key;
        }

        public static string Get(string key, params object[] args)
        {
            var service = LocalizationManager.Service;
            return service != null ? service.Get(key, args) : key;
        }

        public static void ToggleLanguage()
        {
            LocalizationManager.Service?.ToggleLanguage();
        }
    }
}
