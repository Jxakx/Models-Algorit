using UnityEngine;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Fx
{
    //Escucha FloatingDamageTextEvent y pide al factory que muestre el número
    public class DamageNumberController : MonoBehaviour
    {
        [SerializeField] private FloatingDamageNumber prefab;

        private DamageNumberFactory _factory;

        private void Awake()
        {
            _factory = new DamageNumberFactory(prefab, transform);
        }

        private void OnEnable()
        {
            EventManager.Subscribe<FloatingDamageTextEvent>(OnDamage);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<FloatingDamageTextEvent>(OnDamage);
        }

        private void OnDamage(FloatingDamageTextEvent gameEvent)
        {
            _factory.Spawn(gameEvent.Position, gameEvent.Amount, gameEvent.Style);
        }
    }
}
