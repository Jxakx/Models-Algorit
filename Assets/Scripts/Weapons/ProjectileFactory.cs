using UnityEngine;
using ArenaSurvivor.Core.Pooling;
using ArenaSurvivor.Weapons.HitEffects;

namespace ArenaSurvivor.Weapons
{
    public class ProjectileFactory
    {
        private readonly ObjectPool<Projectile> _pool;

        public ProjectileFactory(Projectile prefab, Transform poolParent, int prewarmCount = 30)
        {
            _pool = new ObjectPool<Projectile>(prefab, poolParent, prewarmCount);
        }

        public Projectile Create(Vector2 position, Vector2 direction, WeaponData data, float damage, IHitEffect hitEffect)
        {
            var projectile = _pool.Get(position, Quaternion.identity);
            projectile.Launch(position, direction, data, damage, hitEffect, Release);
            return projectile;
        }

        private void Release(Projectile projectile)
        {
            _pool.Release(projectile);
        }
    }
}
