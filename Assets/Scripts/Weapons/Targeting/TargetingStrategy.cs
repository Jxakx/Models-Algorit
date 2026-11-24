using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Enemies;

namespace ArenaSurvivor.Weapons.Targeting
{
    public abstract class TargetingStrategy : ScriptableObject, ITargetingStrategy
    {
        public abstract Enemy FindTarget(Vector2 origin, float range, IReadOnlyList<Enemy> excluded);

        //¿Este enemigo es candidato? Dentro del alcance y no excluido
        protected static bool IsCandidate(Enemy enemy, Vector2 origin, float rangeSquared,
            IReadOnlyList<Enemy> excluded, out float distanceSquared)
        {
            distanceSquared = ((Vector2)enemy.transform.position - origin).sqrMagnitude;
            if (distanceSquared > rangeSquared)
            {
                return false;
            }

            if (excluded != null)
            {
                for (var i = 0; i < excluded.Count; i++)
                {
                    if (excluded[i] == enemy)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
