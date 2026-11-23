using UnityEngine;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Enemies;

namespace ArenaSurvivor.Fx
{
    public class EnemyDeathParticles : MonoBehaviour
    {
        [SerializeField] private PooledParticleEffect prefab;

        private ParticleEffectFactory _factory;

        private void Awake()
        {
            _factory = new ParticleEffectFactory(prefab, transform);
        }

        private void OnEnable()
        {
            EventManager.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void OnEnemyKilled(EnemyKilledEvent gameEvent)
        {
            _factory.Spawn(gameEvent.Position, gameEvent.Color);
        }
    }
}
