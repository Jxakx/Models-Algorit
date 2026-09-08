using System.Collections.Generic;
using UnityEngine;

namespace ArenaSurvivor.Core.Pooling
{
    /// <summary>
    /// Pool de objetos genérico y reusable (patrón Object Pool): en vez de instanciar y
    /// destruir GameObjects todo el tiempo (caro en CPU y en el Garbage Collector), recicla
    /// instancias desactivadas.
    ///
    /// Sirve para cualquier prefab que tenga un componente T: enemigos, proyectiles, gemas
    /// de XP, etc. Si T implementa IPoolable, el pool avisa al entregarlo/reciclarlo; si no,
    /// igual funciona (activar/desactivar simple).
    /// </summary>
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly int _maxSize;
        private readonly Queue<T> _inactive = new Queue<T>();

        /// <param name="prefab">Prefab a instanciar (debe tener el componente T).</param>
        /// <param name="parent">Transform bajo el cual viven las instancias (orden en Hierarchy).</param>
        /// <param name="initialSize">Cuántas instancias precrear para evitar hitches al arrancar.</param>
        /// <param name="maxSize">Tope de instancias inactivas guardadas (0 = sin límite).</param>
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
