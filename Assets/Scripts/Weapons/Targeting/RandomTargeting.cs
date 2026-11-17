using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Enemies;

namespace ArenaSurvivor.Weapons.Targeting
{
    /// <summary>Apunta a cualquier enemigo al azar dentro del alcance (reparte el daño sin patrón).</summary>
    [CreateAssetMenu(fileName = "Targeting_Random", menuName = "Arena Survivor/Targeting/Random")]
    public class RandomTargeting : TargetingStrategy
    {
        public override Enemy FindTarget(Vector2 origin, float range, IReadOnlyList<Enemy> excluded)
        {
            Enemy chosen = null;
            var candidates = 0;
            var rangeSquared = range * range;

            // Reservoir sampling: elige uniformemente entre N candidatos sin armar una lista —
            // el n-ésimo candidato reemplaza al elegido con probabilidad 1/n.
            var enemies = EnemyRegistry.ActiveEnemies;
            for (var i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                if (!IsCandidate(enemy, origin, rangeSquared, excluded, out _))
                {
                    continue;
                }

                candidates++;
                if (Random.Range(0, candidates) == 0)
                {
                    chosen = enemy;
                }
            }

            return chosen;
        }
    }
}
