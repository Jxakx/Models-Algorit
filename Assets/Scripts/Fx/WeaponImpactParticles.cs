using UnityEngine;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Fx
{
    public class WeaponImpactParticles : MonoBehaviour
    {
        [SerializeField] private PooledParticleEffect prefab;
        [SerializeField] private Color color = Color.white;

        private ParticleEffectFactory _factory;

        private void Awake()
        {
            _factory = new ParticleEffectFactory(prefab, transform);
        }

        private void OnEnable()
        {
            EventManager.Subscribe<WeaponImpactEvent>(OnWeaponImpact);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<WeaponImpactEvent>(OnWeaponImpact);
        }

        private void OnWeaponImpact(WeaponImpactEvent gameEvent)
        {
            _factory.Spawn(gameEvent.Position, color);
        }
    }
}
