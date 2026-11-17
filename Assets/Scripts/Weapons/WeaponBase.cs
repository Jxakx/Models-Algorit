using UnityEngine;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Player;
using ArenaSurvivor.Weapons.HitEffects;

namespace ArenaSurvivor.Weapons
{
    /// <summary>
    /// Base común de toda arma: maneja el cooldown y el nivel, y delega el "cómo dispara"
    /// a la subclase concreta (Template Method). Agregar un arma nueva es heredar de esta
    /// clase e implementar Fire() — no hay que tocar nada del resto del sistema.
    ///
    /// También arma la cadena de efectos de golpe del arma (Decorator): empieza por el daño
    /// base y le va envolviendo encima solo los efectos que el arma tiene configurados.
    /// </summary>
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

        /// <summary>Lo que pasa cuando esta arma conecta un golpe (daño + los decoradores que tenga).</summary>
        public IHitEffect HitEffect => _hitEffect ??= BuildHitEffect();

        protected virtual void Awake()
        {
            _ownerStats = GetComponent<PlayerStats>();
        }

        protected virtual void Start()
        {
            // Un arma que arranca desactivada (todavía no desbloqueada) no llega a este Start —
            // Unity solo lo llama la primera vez que el componente está habilitado. Por eso
            // este mismo Start sirve para avisarle al HUD "esta arma ya existe" tanto para la
            // inicial (activa desde el arranque) como para una desbloqueada más tarde en
            // LevelUpManager (weapon.enabled = true). Se usa Start y no OnEnable para evitar
            // la misma condición de carrera que tuvo la barra de vida: Start recién corre
            // después de que TODOS los OnEnable de la escena ya pasaron, así que el HUD de
            // armas ya está suscripto cuando esto se publica.
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

        // El evento lleva la CLAVE de localización del nombre (no el texto ya traducido):
        // cada oyente la traduce al idioma actual cuando dibuja, y la vuelve a traducir si
        // el jugador cambia de idioma.
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

        /// <summary>Se invoca automáticamente cada vez que se cumple el cooldown.</summary>
        protected virtual void Fire()
        {
        }

        // Decorator: se parte del efecto simple (solo daño) y se envuelve con cada efecto
        // activo. El orden de envoltura define el orden de ejecución: el último en envolver
        // es el primero en actuar, y el daño (el más interno) siempre va al final.
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
