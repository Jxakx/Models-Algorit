using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Core.Pooling;

namespace ArenaSurvivor.Enemies
{
    /// <summary>
    /// Único punto del proyecto que "crea" enemigos: decide, a partir de un EnemyData, si
    /// reusa una instancia inactiva del pool o instancia una nueva, y la inicializa.
    /// Nadie más debería llamar Instantiate sobre el prefab de Enemy directamente.
    ///
    /// Guarda un pool POR CADA EnemyData (por cada flyweight): así una instancia reciclada
    /// siempre vuelve a ser del mismo tipo que ya era, y no hay que reconfigurarle el
    /// aspecto cada vez que reaparece.
    /// </summary>
    public class EnemyFactory
    {
        private readonly Enemy _prefab;
        private readonly Transform _poolParent;
        private readonly int _prewarmPerType;
        private readonly Dictionary<EnemyData, ObjectPool<Enemy>> _poolsByType = new Dictionary<EnemyData, ObjectPool<Enemy>>();

        public EnemyFactory(Enemy prefab, Transform poolParent, int prewarmCount = 20)
        {
            _prefab = prefab;
            _poolParent = poolParent;
            _prewarmPerType = Mathf.Max(1, prewarmCount / 3);
        }

        public Enemy Create(EnemyData data, Vector2 position, Transform target)
        {
            var enemy = PoolFor(data).Get(position, Quaternion.identity);
            enemy.Initialize(data, target, ReturnToPool);
            return enemy;
        }

        private ObjectPool<Enemy> PoolFor(EnemyData data)
        {
            if (!_poolsByType.TryGetValue(data, out var pool))
            {
                pool = new ObjectPool<Enemy>(_prefab, _poolParent, _prewarmPerType);
                _poolsByType[data] = pool;
            }

            return pool;
        }

        private void ReturnToPool(Enemy enemy)
        {
            // Data nunca es null acá: el enemigo solo se devuelve después de haber sido
            // inicializado con un EnemyData.
            PoolFor(enemy.Data).Release(enemy);
        }
    }
}
