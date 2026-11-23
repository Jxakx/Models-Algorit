using System;
using UnityEngine;

namespace ArenaSurvivor.Core.Combat
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;

        [Tooltip("Segundos de invulnerabilidad tras recibir daño. 0 = sin invulnerabilidad " +
                 "(los enemigos usan 0 por defecto; el Player la usa para que varios enemigos " +
                 "tocándolo a la vez no lo maten en un solo instante).")]
        [SerializeField] private float invulnerabilityDuration = 0f;

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }
        public bool IsAlive => CurrentHealth > 0f;

        //Se dispara con (vidaActual, vidaMaxima) cada vez que la vida cambia
        public event Action<float, float> HealthChanged;

        //Se dispara una sola vez, cuando la vida llega a 0
        public event Action Died;

        private float _invulnerableUntil;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive || amount <= 0f)
            {
                return;
            }

            if (Time.time < _invulnerableUntil)
            {
                return;
            }

            CurrentHealth = Mathf.Max(CurrentHealth - amount, 0f);
            HealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (invulnerabilityDuration > 0f)
            {
                _invulnerableUntil = Time.time + invulnerabilityDuration;
            }

            if (CurrentHealth <= 0f)
            {
                Died?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            if (!IsAlive || amount <= 0f)
            {
                return;
            }

            CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        //Redefine la vida máxima y revive con la vida al tope. Pensado para el Enemy: cada vez que el pool lo recicla con un enemyData distinto, se reconfigura en vez de crear un health nuevo.
        public void ConfigureMaxHealth(float newMaxHealth)
        {
            maxHealth = Mathf.Max(newMaxHealth, 1f);
            CurrentHealth = maxHealth;
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}
