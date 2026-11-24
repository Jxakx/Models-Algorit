using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Localization
{
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string key;

        private Text _text;

        public string Key
        {
            get => key;
            set
            {
                key = value;
                Refresh();
            }
        }

        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        private void OnEnable()
        {
            EventManager.Subscribe<LanguageChangedEvent>(OnLanguageChanged);
            Refresh();
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<LanguageChangedEvent>(OnLanguageChanged);
        }

        private void OnLanguageChanged(LanguageChangedEvent gameEvent) => Refresh();

        private void Refresh()
        {
            if (_text == null || string.IsNullOrEmpty(key))
            {
                return;
            }

            _text.text = Localizer.Get(key);
        }
    }
}
