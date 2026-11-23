using System;
using UnityEngine;
using ArenaSurvivor.Core.Combat;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Core.Flyweight;
using ArenaSurvivor.Core.Pooling;
using ArenaSurvivor.Enemies.Movement;
using ArenaSurvivor.Fx;

namespace ArenaSurvivor.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Health))]
    public class Enemy : MonoBehaviour, IPoolable, IKnockbackable, ISlowable
    {
        private const float ContactDamageCooldown = 1f;

        [Tooltip("Qué tan rápido se apaga el empujón que recibe este enemigo al ser golpeado.")]
        [SerializeField] private float knockbackDamping = 8f;

        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private Health _health;
        private SquashStretch _squashStretch;

        // Flyweight: estado intrínseco COMPARTIDO (solo lectura).
        private EnemyData _data;
        private bool _flyweightAcquired;

        // Estado extrínseco: propio de esta instancia.
        private Transform _target;
        private Action<Enemy> _releaseToPool;
        private float _lastContactDamageTime;
        private Vector2 _knockbackVelocity;
        private float _slowMultiplier = 1f;
        private float _slowUntil;
        private float _movementSeed;

        //El flyweight de este enemigo (null mientras está en el pool sin usar)
        public EnemyData Data => _data;

        public float CurrentHealth => _health.CurrentHealth;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _health = GetComponent<Health>();
            _squashStretch = GetComponent<SquashStretch>();

            // Se suscribe una sola vez: Awake corre una única vez en la vida del GameObject,
            // aunque el pool lo recicle muchas veces con SetActive.
            _health.Died += HandleDied;
        }

        //Llamado por el EnemyFactory apenas lo saca del pool
        public void Initialize(EnemyData data, Transform target, Action<Enemy> releaseToPool)
        {
            _target = target;
            _releaseToPool = releaseToPool;

            ReleaseFlyweight();
            _data = FlyweightFactory<EnemyData>.Acquire(data);
            _flyweightAcquired = _data != null;

            // El aspecto (color y escala) sale del flyweight y se aplica en CADA spawn: otros
            // componentes (HitFlash, SquashStretch) también tocan el color y la escala del
            // sprite, así que no se puede asumir que una instancia reciclada lo conserva bien
            _spriteRenderer.color = _data.Color;
            transform.localScale = Vector3.one * _data.Scale;

            // El "pop" de aparición va en cada spawn, con la escala definitiva del tipo
            _squashStretch?.Play(Vector3.one * _data.Scale);

            // La vida actual es extrínseca: cada spawn arranca con la vida máxima del tipo
            _health.ConfigureMaxHealth(_data.MaxHealth);
        }

        public void OnSpawn()
        {
            _lastContactDamageTime = float.NegativeInfinity;
            _knockbackVelocity = Vector2.zero;
            _slowMultiplier = 1f;
            _slowUntil = 0f;
            _movementSeed = UnityEngine.Random.value;
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
            ReleaseFlyweight();
        }

        private void ReleaseFlyweight()
        {
            if (!_flyweightAcquired)
            {
                return;
            }

            FlyweightFactory<EnemyData>.Release(_data);
            _flyweightAcquired = false;
        }

        public void ApplyKnockback(Vector2 direction, float force)
        {
            _knockbackVelocity = direction.normalized * force;
        }

        public void ApplySlow(float speedMultiplier, float duration)
        {
            _slowMultiplier = Mathf.Clamp(speedMultiplier, 0.05f, 1f);
            _slowUntil = Time.time + duration;
        }

        private void FixedUpdate()
        {
            if (_target == null || _data == null)
            {
                return;
            }

            // Strategy: el Enemy no sabe CÓMO se mueve, solo le pide la velocidad a la estrategia de su tipo
            var speed = _data.MoveSpeed * (Time.time < _slowUntil ? _slowMultiplier : 1f);
            var context = new MovementContext(_rigidbody.position, _target.position, speed, Time.time, _movementSeed);
            var chaseVelocity = _data.Movement.CalculateVelocity(context);

            _rigidbody.linearVelocity = chaseVelocity + _knockbackVelocity;
            _knockbackVelocity = Vector2.Lerp(_knockbackVelocity, Vector2.zero, knockbackDamping * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other) => TryDealContactDamage(other);

        private void OnTriggerStay2D(Collider2D other) => TryDealContactDamage(other);

        private void TryDealContactDamage(Collider2D other)
        {
            if (_data == null || !other.CompareTag("Player"))
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

            damageable.TakeDamage(_data.ContactDamage);
            _lastContactDamageTime = Time.time;
        }

        private void HandleDied()
        {
            if (_data != null)
            {
                EventManager.Publish(new EnemyKilledEvent(transform.position, _data.ExperienceReward, _data.Color));
            }

            _releaseToPool?.Invoke(this);
        }
    }
}
