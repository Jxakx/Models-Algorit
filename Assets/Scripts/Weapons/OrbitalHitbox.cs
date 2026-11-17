using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Core.Combat;
using ArenaSurvivor.Enemies;
using ArenaSurvivor.Weapons.HitEffects;

namespace ArenaSurvivor.Weapons
{
    /// <summary>
    /// Collider hijo del OrbitalWeapon: aplica el efecto de golpe del arma con un cooldown
    /// por enemigo tocado.
    /// </summary>
    public class OrbitalHitbox : MonoBehaviour
    {
        private const float HitCooldown = 0.5f;

        private OrbitalWeapon _owner;
        private readonly Dictionary<Enemy, float> _lastHitTimes = new Dictionary<Enemy, float>();

        public void SetOwner(OrbitalWeapon owner)
        {
            _owner = owner;
        }

        private void OnTriggerEnter2D(Collider2D other) => TryHit(other);

        private void OnTriggerStay2D(Collider2D other) => TryHit(other);

        private void TryHit(Collider2D other)
        {
            if (_owner == null || other.CompareTag("Player"))
            {
                return;
            }

            if (!other.TryGetComponent<Enemy>(out var enemy))
            {
                return;
            }

            if (!other.TryGetComponent<IDamageable>(out _))
            {
                return;
            }

            if (_lastHitTimes.TryGetValue(enemy, out var lastHitTime) && Time.time - lastHitTime < HitCooldown)
            {
                return;
            }

            var point = (Vector2)enemy.transform.position;
            var direction = (point - (Vector2)_owner.transform.position).normalized;

            _owner.HitEffect.Apply(new HitContext(enemy, point, direction, _owner.CurrentDamage));
            _lastHitTimes[enemy] = Time.time;
        }
    }
}
