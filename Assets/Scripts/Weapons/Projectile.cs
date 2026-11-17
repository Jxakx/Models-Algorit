using System;
using UnityEngine;
using ArenaSurvivor.Core.Combat;
using ArenaSurvivor.Core.Flyweight;
using ArenaSurvivor.Core.Pooling;
using ArenaSurvivor.Weapons.HitEffects;

namespace ArenaSurvivor.Weapons
{
    /// <summary>
    /// Proyectil pooleado: vuela en línea recta, aplica el efecto de golpe de su arma al
    /// primer IDamageable que no sea el Player, y se libera solo al pool (por impacto o por
    /// vencerse su tiempo de vida).
    ///
    /// Es un contexto del Flyweight: velocidad y tiempo de vida las lee del WeaponData
    /// compartido por todos los proyectiles del arma, en vez de copiarlos en cada bala.
    /// Lo único propio de cada proyectil es su dirección, su daño y su cronómetro.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour, IPoolable
    {
        private static readonly IHitEffect DefaultEffect = new DamageHitEffect();

        private Rigidbody2D _rigidbody;
        private Action<Projectile> _releaseToPool;
        private WeaponData _data;
        private IHitEffect _hitEffect;
        private Vector2 _direction;
        private float _damage;
        private float _timer;
        private bool _released;
        private bool _flyweightAcquired;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Launch(Vector2 position, Vector2 direction, WeaponData data, float damage, IHitEffect hitEffect,
            Action<Projectile> releaseToPool)
        {
            ReleaseFlyweight();
            _data = FlyweightFactory<WeaponData>.Acquire(data);
            _flyweightAcquired = _data != null;

            transform.position = position;
            _direction = direction.normalized;
            _rigidbody.linearVelocity = _direction * _data.ProjectileSpeed;
            _damage = damage;
            _hitEffect = hitEffect ?? DefaultEffect;
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

        private void OnDisable()
        {
            ReleaseFlyweight();
        }

        private void ReleaseFlyweight()
        {
            if (!_flyweightAcquired)
            {
                return;
            }

            FlyweightFactory<WeaponData>.Release(_data);
            _flyweightAcquired = false;
        }

        private void Update()
        {
            if (_data == null)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer >= _data.ProjectileLifetime)
            {
                ReleaseSelf();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_released || other.CompareTag("Player"))
            {
                return;
            }

            if (!other.TryGetComponent<IDamageable>(out _))
            {
                return;
            }

            _hitEffect.Apply(new HitContext(other, transform.position, _direction, _damage));
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
