using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Localization;
using ArenaSurvivor.Player;

namespace ArenaSurvivor.UI
{
    /// <summary>
    /// Barra de XP + texto de nivel. Escucha PlayerXpChangedEvent (Observer) y
    /// LanguageChangedEvent (para volver a escribir "Nv."/"Lv." en el idioma nuevo).
    /// </summary>
    public class XpBarUI : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private Text levelText;

        private int _level = 1;

        private void OnEnable()
        {
            EventManager.Subscribe<PlayerXpChangedEvent>(OnXpChanged);
            EventManager.Subscribe<LanguageChangedEvent>(OnLanguageChanged);
            RedrawLevel();
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<PlayerXpChangedEvent>(OnXpChanged);
            EventManager.Unsubscribe<LanguageChangedEvent>(OnLanguageChanged);
        }

        private void OnXpChanged(PlayerXpChangedEvent gameEvent)
        {
            fillImage.fillAmount = gameEvent.XpToNextLevel <= 0
                ? 0f
                : Mathf.Clamp01((float)gameEvent.CurrentXp / gameEvent.XpToNextLevel);

            _level = gameEvent.Level;
            RedrawLevel();
        }

        private void OnLanguageChanged(LanguageChangedEvent gameEvent) => RedrawLevel();

        private void RedrawLevel()
        {
            if (levelText != null)
            {
                levelText.text = Localizer.Get(LocKeys.HudLevel, _level);
            }
        }
    }
}
