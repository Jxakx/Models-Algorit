using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Enemies;

namespace ArenaSurvivor.Weapons.Targeting
{
    public interface ITargetingStrategy
    {
        /// <param name="origin">Desde dónde se mide el alcance.</param>
        /// <param name="range">Alcance máximo.</param>
        /// <param name="excluded">Enemigos que no se pueden elegir (null = ninguno). Lo usa la cadena eléctrica.</param>
        Enemy FindTarget(Vector2 origin, float range, IReadOnlyList<Enemy> excluded);
    }
}
