using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Enemies;

namespace ArenaSurvivor.Weapons.Targeting
{
    //Apunta al enemigo más cercano
    [CreateAssetMenu(fileName = "Targeting_Nearest", menuName = "Arena Survivor/Targeting/Nearest")]
    public class NearestTargeting : TargetingStrategy
    {
        public override Enemy FindTarget(Vector2 origin, float range, IReadOnlyList<Enemy> excluded)
        {
            Enemy best = null;
            var bestDistanceSquared = float.MaxValue;
            var rangeSquared = range * range;

            var enemies = EnemyRegistry.ActiveEnemies;
            for (var i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                if (IsCandidate(enemy, origin, rangeSquared, excluded, out var distanceSquared)
                    && distanceSquared < bestDistanceSquared)
                {
                    bestDistanceSquared = distanceSquared;
                    best = enemy;
                }
            }

            return best;
        }
    }
}
