using UnityEngine;
using ArenaSurvivor.Core.Pooling;

namespace ArenaSurvivor.Weapons
{
    /// <summary>Crea/recicla proyectiles vía ObjectPool — mismo patrón que EnemyFactory.</summary>
    public class ProjectileFactory
    {
        private readonly ObjectPool<Projectile> _pool;

        public ProjectileFactory(Projectile prefab, Transform poolParent, int prewarmCount = 30)
        {
            _pool = new ObjectPool<Projectile>(prefab, poolParent, prewarmCount);
        }

        public Projectile Create(Vector2 position, Vector2 direction, float speed, float damage, float lifetime)
        {
            var projectile = _pool.Get(position, Quaternion.identity);
            projectile.Launch(position, direction, speed, damage, lifetime, Release);
            return projectile;
        }

        private void Release(Projectile projectile)
        {
            _pool.Release(projectile);
        }
    }
}
