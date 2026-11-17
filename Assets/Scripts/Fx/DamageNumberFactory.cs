using UnityEngine;
using ArenaSurvivor.Core.Pooling;

namespace ArenaSurvivor.Fx
{
    /// <summary>Crea/recicla números de daño flotantes vía ObjectPool — mismo patrón que EnemyFactory.</summary>
    public class DamageNumberFactory
    {
        private readonly ObjectPool<FloatingDamageNumber> _pool;

        public DamageNumberFactory(FloatingDamageNumber prefab, Transform poolParent, int prewarmCount = 20)
        {
            _pool = new ObjectPool<FloatingDamageNumber>(prefab, poolParent, prewarmCount);
        }

        public void Spawn(Vector2 position, float amount, DamageNumberStyle style)
        {
            var number = _pool.Get(position, Quaternion.identity);
            number.Show(amount, style, Release);
        }

        private void Release(FloatingDamageNumber number)
        {
            _pool.Release(number);
        }
    }
}
