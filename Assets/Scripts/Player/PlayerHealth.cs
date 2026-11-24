using UnityEngine;
using ArenaSurvivor.Core.Combat;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Player
{
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
