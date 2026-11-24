using UnityEngine;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Fx;

namespace ArenaSurvivor.Weapons
{
    public class ProjectileWeapon : TargetedWeapon
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float spreadAngleDegrees = 10f;

        [Tooltip("No dispara a enemigos más lejos que esto — así las muertes pasan dentro de " +
                 "lo que la cámara muestra, en vez de matar cosas que nunca se llegan a ver.")]
        [SerializeField] private float attackRange = 9f;

        private ProjectileFactory _factory;

        protected override void Awake()
        {
            base.Awake();
            _factory = new ProjectileFactory(projectilePrefab, transform);
        }

        protected override void Fire()
        {
            var target = FindTarget(transform.position, attackRange);
            if (target == null)
            {
                return;
            }

            var baseDirection = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            var damage = CurrentDamage;
            var count = Data.GetProjectileCount(CurrentLevel);

            EventManager.Publish(new WeaponFiredEvent(Data.FireSound));

            for (var i = 0; i < count; i++)
            {
                var offsetIndex = i - (count - 1) / 2f;
                var rotatedDirection = Quaternion.Euler(0f, 0f, offsetIndex * spreadAngleDegrees) * baseDirection;
                _factory.Create(transform.position, rotatedDirection, Data, damage, HitEffect);
            }
        }
    }
}
