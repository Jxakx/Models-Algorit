using System;
using UnityEngine;
using ArenaSurvivor.Core.Combat;
using ArenaSurvivor.Core.Pooling;

namespace ArenaSurvivor.Weapons
{
    /// <summary>
    /// Proyectil pooleado: vuela en línea recta, hace daño al primer IDamageable que no sea
    /// el Player, y se libera solo al pool (por impacto o por vencerse su tiempo de vida).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour, IPoolable
    {
        private Rigidbody2D _rigidbody;
        private Action<Projectile> _releaseToPool;
        private float _damage;
        private float _lifetime;
        private float _timer;
        private bool _released;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Launch(Vector2 position, Vector2 direction, float speed, float damage, float lifetime, Action<Projectile> releaseToPool)
        {
            transform.position = position;
            _rigidbody.linearVelocity = direction.normalized * speed;
            _damage = damage;
            _lifetime = lifetime;
            _releaseToPool = releaseToPool;
        }

        public void OnSpawn()
        {
            _timer = 0f;
            _released = false;
        }

        public void OnDespawn()
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _lifetime)
            {
                ReleaseSelf();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                return;
            }

            if (!other.TryGetComponent<IDamageable>(out var damageable))
            {
                return;
            }

            damageable.TakeDamage(_damage);
            ReleaseSelf();
        }

        private void ReleaseSelf()
        {
            if (_released)
            {
                return;
            }

            _released = true;
            _releaseToPool?.Invoke(this);
        }
    }
}
