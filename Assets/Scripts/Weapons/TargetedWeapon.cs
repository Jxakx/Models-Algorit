using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Core.Flyweight;
using ArenaSurvivor.Enemies;
using ArenaSurvivor.Weapons.Targeting;

namespace ArenaSurvivor.Weapons
{
    public abstract class TargetedWeapon : WeaponBase
    {
        [Header("Targeting (Strategy)")]
        [Tooltip("A quién le apunta esta arma. Vacío = el más cercano.")]
        [SerializeField] private TargetingStrategy targetingStrategy;
        public Enemy FindTarget(Vector2 origin, float range, IReadOnlyList<Enemy> excluded = null)
        {
            return CurrentStrategy().FindTarget(origin, range, excluded);
        }

        private ITargetingStrategy CurrentStrategy()
        {
            return targetingStrategy != null ? targetingStrategy : DefaultStrategy();
        }

        private static ITargetingStrategy DefaultStrategy()
        {
            return FlyweightFactory<TargetingStrategy>.GetOrCreate("default-nearest", () =>
            {
                var nearest = ScriptableObject.CreateInstance<NearestTargeting>();
                nearest.hideFlags = HideFlags.HideAndDontSave;
                return nearest;
            });
        }
    }
}
