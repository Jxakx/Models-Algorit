using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Enemies;

namespace ArenaSurvivor.Weapons.Targeting
{
    /// <summary>Apunta al enemigo con más vida actual dentro del alcance (le saca el tanque al enjambre).</summary>
    [CreateAssetMenu(fileName = "Targeting_Strongest", menuName = "Arena Survivor/Targeting/Strongest")]
    public class StrongestTargeting : TargetingStrategy
    {
        public override Enemy FindTarget(Vector2 origin, float range, IReadOnlyList<Enemy> excluded)
        {
            Enemy best = null;
            var bestHealth = -1f;
            var rangeSquared = range * range;

            var enemies = EnemyRegistry.ActiveEnemies;
            for (var i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                if (IsCandidate(enemy, origin, rangeSquared, excluded, out _) && enemy.CurrentHealth > bestHealth)
                {
                    bestHealth = enemy.CurrentHealth;
                    best = enemy;
                }
            }

            return best;
        }
    }
}
