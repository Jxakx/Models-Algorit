using UnityEngine;
using ArenaSurvivor.Enemies;

namespace ArenaSurvivor.Weapons
{
    /// <summary>Arma base: dispara al enemigo más cercano. Más proyectiles al subir de nivel.</summary>
    public class ProjectileWeapon : WeaponBase
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
            var target = FindNearestEnemy();
            if (target == null)
            {
                return;
            }

            var baseDirection = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            var damage = CurrentDamage;
            var count = Data.GetProjectileCount(CurrentLevel);

            for (var i = 0; i < count; i++)
            {
                var offsetIndex = i - (count - 1) / 2f;
                var rotatedDirection = Quaternion.Euler(0f, 0f, offsetIndex * spreadAngleDegrees) * baseDirection;
                _factory.Create(transform.position, rotatedDirection, Data.ProjectileSpeed, damage, Data.ProjectileLifetime);
            }
        }

        private Enemy FindNearestEnemy()
        {
            Enemy nearest = null;
            var nearestDistanceSquared = float.MaxValue;
            var rangeSquared = attackRange * attackRange;

            foreach (var enemy in EnemyRegistry.ActiveEnemies)
            {
                var distanceSquared = ((Vector2)enemy.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (distanceSquared <= rangeSquared && distanceSquared < nearestDistanceSquared)
                {
                    nearestDistanceSquared = distanceSquared;
                    nearest = enemy;
                }
            }

            return nearest;
        }
    }
}
