using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Player;

namespace ArenaSurvivor.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private Text hpText;

        private void Awake()
        {
            fillImage.fillAmount = 1f;
        }

        private void OnEnable()
        {
            EventManager.Subscribe<PlayerHealthChangedEvent>(OnPlayerHealthChanged);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<PlayerHealthChangedEvent>(OnPlayerHealthChanged);
        }

        private void OnPlayerHealthChanged(PlayerHealthChangedEvent gameEvent)
        {
            fillImage.fillAmount = gameEvent.MaxHealth <= 0f
                ? 0f
                : Mathf.Clamp01(gameEvent.CurrentHealth / gameEvent.MaxHealth);

            if (hpText != null)
            {
                hpText.text = $"{Mathf.CeilToInt(gameEvent.CurrentHealth)}/{Mathf.CeilToInt(gameEvent.MaxHealth)}";
            }
        }
    }
}
