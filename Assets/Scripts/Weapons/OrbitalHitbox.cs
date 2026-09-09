using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Core.Combat;
using ArenaSurvivor.Enemies;

namespace ArenaSurvivor.Weapons
{
    /// <summary>Collider hijo del OrbitalWeapon: aplica daño con un cooldown por enemigo tocado.</summary>
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
            if (other.CompareTag("Player"))
            {
                return;
            }

            if (!other.TryGetComponent<Enemy>(out var enemy))
            {
                return;
            }

            if (!other.TryGetComponent<IDamageable>(out var damageable))
            {
                return;
            }

            if (_lastHitTimes.TryGetValue(enemy, out var lastHitTime) && Time.time - lastHitTime < HitCooldown)
            {
                return;
            }

            damageable.TakeDamage(_owner.CurrentDamage);
            _lastHitTimes[enemy] = Time.time;
        }
    }
}
