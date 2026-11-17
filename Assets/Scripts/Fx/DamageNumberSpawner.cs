using UnityEngine;
using ArenaSurvivor.Core.Combat;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Core.Flyweight;

namespace ArenaSurvivor.Fx
{
    /// <summary>
    /// Publica un FloatingDamageTextEvent cada vez que SU PROPIO Health pierde vida. Un
    /// componente igual va en el Player (estilo rojo) y en el prefab de Enemy (estilo
    /// blanco) — cada uno decide su estilo, ninguno sabe nada de pooling ni de cómo se
    /// dibuja el número.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class DamageNumberSpawner : MonoBehaviour
    {
        [Tooltip("Estilo compartido (flyweight) del número. Vacío = uno blanco por defecto.")]
        [SerializeField] private DamageNumberStyle style;

        private Health _health;
        private float _lastKnownHealth;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.HealthChanged += HandleHealthChanged;
        }

        private void Start()
        {
            _lastKnownHealth = _health.CurrentHealth;
        }

        private void OnDisable()
        {
            _health.HealthChanged -= HandleHealthChanged;
        }

        private void HandleHealthChanged(float current, float max)
        {
            if (current < _lastKnownHealth)
            {
                EventManager.Publish(new FloatingDamageTextEvent(transform.position, _lastKnownHealth - current, CurrentStyle()));
            }

            _lastKnownHealth = current;
        }

        private DamageNumberStyle CurrentStyle()
        {
            return style != null ? style : DefaultStyle();
        }

        private static DamageNumberStyle DefaultStyle()
        {
            return FlyweightFactory<DamageNumberStyle>.GetOrCreate("default-white", () =>
            {
                var created = ScriptableObject.CreateInstance<DamageNumberStyle>();
                created.hideFlags = HideFlags.HideAndDontSave;
                return created;
            });
        }
    }
}
