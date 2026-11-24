using UnityEngine;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Player;
using ArenaSurvivor.Weapons.HitEffects;

namespace ArenaSurvivor.Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected WeaponData data;

        [Header("Efectos de golpe (Decorator)")]
        [Tooltip("Fuerza del empujón que deja el golpe. 0 = sin empujón.")]
        [SerializeField] private float knockbackForce = 3f;

        [Tooltip("Multiplicador de velocidad que deja el golpe (1 = no ralentiza).")]
        [SerializeField, Range(0.05f, 1f)] private float slowMultiplier = 1f;

        [Tooltip("Segundos que dura la ralentización. 0 = no ralentiza.")]
        [SerializeField] private float slowDuration;

        [Tooltip("Publica un impacto en cada golpe (de ahí salen las chispas y el sonido).")]
        [SerializeField] private bool impactSparks = true;

        private float _cooldownTimer;
        private PlayerStats _ownerStats;
        private IHitEffect _hitEffect;

        public WeaponData Data => data;
        public int CurrentLevel { get; private set; } = 1;
        public bool IsMaxLevel => CurrentLevel >= data.MaxLevel;

        public float CurrentDamage =>
            data.GetDamage(CurrentLevel) * (_ownerStats != null ? _ownerStats.DamageMultiplier : 1f);
        public IHitEffect HitEffect => _hitEffect ??= BuildHitEffect();

        protected virtual void Awake()
        {
            _ownerStats = GetComponent<PlayerStats>();
        }

        protected virtual void Start()
        {
            PublishChanged();
        }

        public void LevelUp()
        {
            if (!IsMaxLevel)
            {
                CurrentLevel++;
                PublishChanged();
            }
        }

        private void PublishChanged()
        {
            EventManager.Publish(new WeaponChangedEvent(data.NameKey, CurrentLevel, data.MaxLevel));
        }

        protected virtual void Update()
        {
            _cooldownTimer -= Time.deltaTime;

            if (_cooldownTimer <= 0f)
            {
                _cooldownTimer = data.GetCooldown(CurrentLevel);
                Fire();
            }
        }

        //Se invoca automáticamente cada vez que se cumple el cooldown
        protected virtual void Fire()
        {
        }

        private IHitEffect BuildHitEffect()
        {
            IHitEffect effect = new DamageHitEffect();

            if (knockbackForce > 0f)
            {
                effect = new KnockbackDecorator(effect, knockbackForce);
            }

            if (slowDuration > 0f && slowMultiplier < 1f)
            {
                effect = new SlowDecorator(effect, slowMultiplier, slowDuration);
            }

            if (impactSparks)
            {
                effect = new ImpactSparksDecorator(effect);
            }

            return effect;
        }
    }
}
