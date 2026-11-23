using System.Collections.Generic;
using UnityEngine;

namespace ArenaSurvivor.Core.Pooling
{
    //Pool de objetos genérico y reusable (patrón Object Pool): en vez de instanciar y destruir GameObjects todo el tiempo, recicla instancias desactivadas.
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly int _maxSize;
        private readonly Queue<T> _inactive = new Queue<T>();

        public ObjectPool(T prefab, Transform parent, int initialSize = 0, int maxSize = 0)
        {
            _prefab = prefab;
            _parent = parent;
            _maxSize = maxSize;

            for (var i = 0; i < initialSize; i++)
            {
                var instance = CreateNew();
                instance.gameObject.SetActive(false);
                _inactive.Enqueue(instance);
            }
        }

        public T Get(Vector3 position, Quaternion rotation)
        {
            var instance = _inactive.Count > 0 ? _inactive.Dequeue() : CreateNew();

            instance.transform.SetPositionAndRotation(position, rotation);
            instance.gameObject.SetActive(true);

            if (instance is IPoolable poolable)
            {
                poolable.OnSpawn();
            }

            return instance;
        }

        public void Release(T instance)
        {
            if (instance is IPoolable poolable)
            {
                poolable.OnDespawn();
            }

            instance.gameObject.SetActive(false);

            if (_maxSize > 0 && _inactive.Count >= _maxSize)
            {
                Object.Destroy(instance.gameObject);
                return;
            }

            _inactive.Enqueue(instance);
        }

        private T CreateNew()
        {
            return Object.Instantiate(_prefab, _parent);
        }
    }
}
