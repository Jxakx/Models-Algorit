using System;
using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Localization
{
    [DefaultExecutionOrder(-200)]
    public class LocalizationManager : MonoBehaviour, ILocalizationService
    {
        private const string LanguagePrefKey = "ArenaSurvivor.Language";

        [SerializeField] private LocalizationTable table;
        [SerializeField] private Language defaultLanguage = Language.Spanish;

        private readonly Dictionary<string, LocalizationEntry> _entriesByKey = new Dictionary<string, LocalizationEntry>();

        public static ILocalizationService Service { get; private set; }

        public Language CurrentLanguage { get; private set; }

        private void Awake()
        {
            Service = this;
            BuildIndex();
            CurrentLanguage = (Language)PlayerPrefs.GetInt(LanguagePrefKey, (int)defaultLanguage);
        }

        private void Start()
        {
            EventManager.Publish(new LanguageChangedEvent(CurrentLanguage));
        }

        private void OnDestroy()
        {
            if (Service == (ILocalizationService)this)
            {
                Service = null;
            }
        }

        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            if (_entriesByKey.TryGetValue(key, out var entry))
            {
                var text = entry.GetText(CurrentLanguage);
                if (!string.IsNullOrEmpty(text))
                {
                    return text;
                }
            }

            return key;
        }

        public string Get(string key, params object[] args)
        {
            var text = Get(key);

            try
            {
                return string.Format(text, args);
            }
            catch (FormatException)
            {
                return text;
            }
        }

        public void SetLanguage(Language language)
        {
            if (language == CurrentLanguage)
            {
                return;
            }

            CurrentLanguage = language;
            PlayerPrefs.SetInt(LanguagePrefKey, (int)language);
            PlayerPrefs.Save();

            EventManager.Publish(new LanguageChangedEvent(language));
        }

        public void ToggleLanguage()
        {
            SetLanguage(CurrentLanguage == Language.Spanish ? Language.English : Language.Spanish);
        }

        private void BuildIndex()
        {
            _entriesByKey.Clear();

            if (table == null)
            {
                Debug.LogWarning("[LocalizationManager] No hay LocalizationTable asignada: los textos van a mostrar su clave.");
                return;
            }

            foreach (var entry in table.Entries)
            {
                if (!string.IsNullOrEmpty(entry.key))
                {
                    _entriesByKey[entry.key] = entry;
                }
            }
        }
    }
}
