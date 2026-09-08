using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Player;

namespace ArenaSurvivor.UI
{
    /// <summary>Barra de XP + texto de nivel. Solo escucha PlayerXpChangedEvent (Observer).</summary>
    public class XpBarUI : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private Text levelText;

        private void OnEnable()
        {
            EventManager.Subscribe<PlayerXpChangedEvent>(OnXpChanged);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<PlayerXpChangedEvent>(OnXpChanged);
        }

        private void OnXpChanged(PlayerXpChangedEvent gameEvent)
        {
            fillImage.fillAmount = gameEvent.XpToNextLevel <= 0
                ? 0f
                : Mathf.Clamp01((float)gameEvent.CurrentXp / gameEvent.XpToNextLevel);

            if (levelText != null)
            {
                levelText.text = $"Nv. {gameEvent.Level}";
            }
        }
    }
}
