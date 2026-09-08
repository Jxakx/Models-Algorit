using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Player;

namespace ArenaSurvivor.UI
{
    /// <summary>
    /// Actualiza la barra de vida del jugador. La jerarquía de UI (Canvas, Image de fondo,
    /// Image de relleno) se arma a mano en el Editor; este script solo necesita la referencia
    /// a la Image de relleno para pintar el porcentaje de vida actual.
    ///
    /// No conoce al Player ni a su Health — solo escucha PlayerHealthChangedEvent (Observer).
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private Text hpText;

        private void Awake()
        {
            // Arranca llena a la vista: Awake corre para TODOS los objetos de la escena antes
            // que cualquier OnEnable, así que si PlayerHealth publica su estado inicial antes
            // de que este script llegue a suscribirse (el orden entre OnEnable de objetos
            // distintos no está garantizado), la barra no se queda invisible/vacía esperando
            // el primer golpe — ya arrancó mostrando el 100% real con el que siempre empieza.
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
