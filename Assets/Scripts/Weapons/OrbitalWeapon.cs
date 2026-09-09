using UnityEngine;

namespace ArenaSurvivor.Weapons
{
    /// <summary>
    /// Arma de área: un objeto gira alrededor del jugador y daña por contacto continuo
    /// (con cooldown propio por enemigo, en OrbitalHitbox). No usa el ciclo de cooldown/Fire
    /// de WeaponBase porque su daño es constante, no un disparo puntual.
    /// </summary>
    public class OrbitalWeapon : WeaponBase
    {
        [SerializeField] private Transform orbitVisual;
        [SerializeField] private float orbitRadius = 2f;
        [SerializeField] private float orbitSpeedDegreesPerSecond = 180f;

        private float _angleDegrees;
        private OrbitalHitbox _hitbox;

        protected override void Awake()
        {
            base.Awake();
            _hitbox = orbitVisual.GetComponent<OrbitalHitbox>();
            _hitbox.SetOwner(this);
        }

        protected override void Update()
        {
            _angleDegrees += orbitSpeedDegreesPerSecond * Time.deltaTime;
            var radians = _angleDegrees * Mathf.Deg2Rad;
            orbitVisual.localPosition = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * orbitRadius;
        }
    }
}
