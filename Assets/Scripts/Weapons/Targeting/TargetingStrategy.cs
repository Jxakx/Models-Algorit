using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Enemies;

namespace ArenaSurvivor.Weapons.Targeting
{
    /// <summary>
    /// Base ScriptableObject de las estrategias de targeting: cada criterio es un asset que
    /// se arrastra al arma en el Inspector. Trae el filtro común (dentro del alcance y no
    /// excluido) para que cada estrategia concreta solo escriba su criterio de elección.
    /// Son stateless: un mismo asset lo comparten todas las armas que lo usan.
    /// </summary>
    public abstract class TargetingStrategy : ScriptableObject, ITargetingStrategy
    {
        public abstract Enemy FindTarget(Vector2 origin, float range, IReadOnlyList<Enemy> excluded);

        /// <summary>¿Este enemigo es candidato? Dentro del alcance y no excluido.</summary>
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
