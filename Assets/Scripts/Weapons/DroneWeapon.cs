using UnityEngine;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Fx;

namespace ArenaSurvivor.Weapons
{
    /// <summary>
    /// Dron acompañante: flota cerca del jugador y dispara solo, de
    /// forma independiente, al enemigo que elija su estrategia de targeting
    /// </summary>
    public class DroneWeapon : TargetedWeapon
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform droneVisual;
        [SerializeField] private float range = 10f;

        private ProjectileFactory _factory;

        protected override void Awake()
        {
            base.Awake();
            _factory = new ProjectileFactory(projectilePrefab, transform);

            // El movimiento del dron también usa la estrategia de targeting del arma.
            if (droneVisual != null && droneVisual.TryGetComponent<DroneMovement>(out var movement))
            {
                movement.SetOwner(this);
            }
        }

        protected override void Fire()
        {
            if (droneVisual == null)
            {
                return;
            }

            var origin = (Vector2)droneVisual.position;
            var target = FindTarget(origin, range);
            if (target == null)
            {
                return;
            }

            EventManager.Publish(new WeaponFiredEvent(Data.FireSound));

            var direction = ((Vector2)target.transform.position - origin).normalized;
            _factory.Create(origin, direction, Data, CurrentDamage, HitEffect);
        }

        private void OnEnable()
        {
            if (droneVisual != null)
            {
                droneVisual.gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            if (droneVisual != null)
            {
                droneVisual.gameObject.SetActive(false);
            }
        }
    }
}
