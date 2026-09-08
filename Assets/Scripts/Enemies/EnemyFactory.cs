using UnityEngine;
using ArenaSurvivor.Core.Pooling;

namespace ArenaSurvivor.Enemies
{
    /// <summary>
    /// Único punto del proyecto que "crea" enemigos: decide, a partir de un EnemyData, si
    /// reusa una instancia inactiva del pool o instancia una nueva, y la inicializa.
    /// Nadie más debería llamar Instantiate sobre el prefab de Enemy directamente.
    /// </summary>
    public class EnemyFactory
    {
        private readonly ObjectPool<Enemy> _pool;

        public EnemyFactory(Enemy prefab, Transform poolParent, int prewarmCount = 20)
        {
            _pool = new ObjectPool<Enemy>(prefab, poolParent, prewarmCount);
        }

        public Enemy Create(EnemyData data, Vector2 position, Transform target)
        {
            var enemy = _pool.Get(position, Quaternion.identity);
            enemy.Initialize(data, target, ReturnToPool);
            return enemy;
        }

        private void ReturnToPool(Enemy enemy)
        {
            _pool.Release(enemy);
        }
    }
}
