using UnityEngine;
using ArenaSurvivor.Core.Combat;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Player
{
    /// <summary>
    /// Traduce los eventos LOCALES del Health del jugador en eventos GLOBALES del EventManager.
    /// Es la única pieza que sabe "este Health en particular es el del jugador" — así Health
    /// se mantiene genérico (lo reusan los enemigos) y el resto del juego (HUD, Game Over)
    /// no necesita ninguna referencia directa al Player.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class PlayerHealth : MonoBehaviour
    {
        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.HealthChanged += HandleHealthChanged;
            _health.Died += HandleDied;
        }

        private void Start()
        {
            // Start corre recién después de que TODOS los Awake de la escena ya
            // terminaron (garantía de Unity), incluido Health.Awake() de este mismo
            // GameObject. Publicar el estado inicial acá (y no en OnEnable) evita una
            // condición de carrera entre componentes: OnEnable de PlayerHealth podía
            // ejecutarse antes de que Health.Awake() inicializara CurrentHealth,
            // publicando por error un 0/100 que el HUD mostraba hasta el primer golpe.
            HandleHealthChanged(_health.CurrentHealth, _health.MaxHealth);
        }

        private void OnDisable()
        {
            _health.HealthChanged -= HandleHealthChanged;
            _health.Died -= HandleDied;
        }

        private void HandleHealthChanged(float current, float max)
        {
            EventManager.Publish(new PlayerHealthChangedEvent(current, max));
        }

        private void HandleDied()
        {
            EventManager.Publish(new PlayerDiedEvent());
        }
    }
}
