namespace ArenaSurvivor.Localization
{
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
