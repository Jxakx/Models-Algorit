using System;
using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Localization
{
    /// <summary>
    /// Administra el idioma del juego (Localization Manager). Guarda la tabla de textos
    /// indexada por clave, sabe cuál es el idioma actual, lo recuerda entre partidas
    /// (PlayerPrefs) y avisa por el EventManager cuando cambia, para que cada texto de
    /// pantalla se redibuje solo. No conoce a ningún texto ni botón: se limita a responder
    /// "qué dice esta clave" y a anunciar el cambio.
    ///
    /// Corre antes que el resto de la escena (ExecutionOrder negativo) para que el servicio
    /// ya exista cuando los demás scripts piden su primer texto en OnEnable.
    /// </summary>
    [DefaultExecutionOrder(-200)]
    public class LocalizationManager : MonoBehaviour, ILocalizationService
    {
        private const string LanguagePrefKey = "ArenaSurvivor.Language";

        [SerializeField] private LocalizationTable table;
        [SerializeField] private Language defaultLanguage = Language.Spanish;

        private readonly Dictionary<string, LocalizationEntry> _entriesByKey = new Dictionary<string, LocalizationEntry>();

        /// <summary>El servicio de la escena actual. Null si no hay un manager activo (los textos muestran su clave).</summary>
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
            // Algunos textos se dibujaron en su OnEnable antes de que el servicio tuviera todo
            // listo: este aviso inicial los obliga a redibujarse con el idioma real.
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

            // Clave sin traducción: se muestra la clave misma, así el hueco se nota en pantalla.
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
