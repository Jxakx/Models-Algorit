using UnityEngine;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Player;

namespace ArenaSurvivor.Weapons
{
    /// <summary>
    /// Base común de toda arma: maneja el cooldown y el nivel, y delega el "cómo dispara"
    /// a la subclase concreta (Template Method). Agregar un arma nueva es heredar de esta
    /// clase e implementar Fire() — no hay que tocar nada del resto del sistema.
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected WeaponData data;

        private float _cooldownTimer;
        private PlayerStats _ownerStats;

        public WeaponData Data => data;
        public int CurrentLevel { get; private set; } = 1;
        public bool IsMaxLevel => CurrentLevel >= data.MaxLevel;

        public float CurrentDamage =>
            data.GetDamage(CurrentLevel) * (_ownerStats != null ? _ownerStats.DamageMultiplier : 1f);

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

        private void PublishChanged()
        {
            EventManager.Publish(new WeaponChangedEvent(data.WeaponName, CurrentLevel, data.MaxLevel));
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
    }
}
