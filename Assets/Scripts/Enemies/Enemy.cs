using System;
using UnityEngine;
using ArenaSurvivor.Core.Combat;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Core.Pooling;

namespace ArenaSurvivor.Enemies
{
    /// <summary>
    /// Comportamiento de un enemigo: perseguir al target y hacer daño de contacto. Las
    /// estadísticas (vida, velocidad, daño, color) las recibe de un EnemyData en Initialize —
    /// el mismo prefab sirve para todos los "tipos" de enemigo, solo cambian los datos.
    /// Se recicla vía ObjectPool (IPoolable) en vez de instanciarse/destruirse cada vez.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Health))]
    public class Enemy : MonoBehaviour, IPoolable
    {
        private const float ContactDamageCooldown = 1f;

        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private Health _health;

        private Transform _target;
        private Action<Enemy> _releaseToPool;
        private float _moveSpeed;
        private float _contactDamage;
        private int _experienceReward;
        private float _lastContactDamageTime;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _health = GetComponent<Health>();

            // Se suscribe una sola vez: Awake corre una única vez en la vida del GameObject,
            // aunque el pool lo recicle muchas veces con SetActive.
            _health.Died += HandleDied;
        }

        /// <summary>Llamado por el EnemyFactory apenas lo saca del pool.</summary>
        public void Initialize(EnemyData data, Transform target, Action<Enemy> releaseToPool)
        {
            _target = target;
            _releaseToPool = releaseToPool;
            _moveSpeed = data.MoveSpeed;
            _contactDamage = data.ContactDamage;
            _experienceReward = data.ExperienceReward;

            _spriteRenderer.color = data.Color;
            transform.localScale = Vector3.one * data.Scale;

            _health.ConfigureMaxHealth(data.MaxHealth);
        }

        public void OnSpawn()
        {
            _lastContactDamageTime = float.NegativeInfinity;
        }

        public void OnDespawn()
        {
            _target = null;
            _rigidbody.linearVelocity = Vector2.zero;
        }

        private void OnEnable()
        {
            EnemyRegistry.Register(this);
        }

        private void OnDisable()
        {
            EnemyRegistry.Unregister(this);
        }

        private void FixedUpdate()
        {
            if (_target == null)
            {
                return;
            }

            var direction = ((Vector2)_target.position - _rigidbody.position).normalized;
            _rigidbody.linearVelocity = direction * _moveSpeed;
        }

        private void OnTriggerEnter2D(Collider2D other) => TryDealContactDamage(other);

        private void OnTriggerStay2D(Collider2D other) => TryDealContactDamage(other);

        private void TryDealContactDamage(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            if (Time.time - _lastContactDamageTime < ContactDamageCooldown)
            {
                return;
            }

            if (!other.TryGetComponent<IDamageable>(out var damageable))
            {
                return;
            }

            damageable.TakeDamage(_contactDamage);
            _lastContactDamageTime = Time.time;
        }

        private void HandleDied()
        {
            EventManager.Publish(new EnemyKilledEvent(transform.position, _experienceReward));
            _releaseToPool?.Invoke(this);
        }
    }
}
