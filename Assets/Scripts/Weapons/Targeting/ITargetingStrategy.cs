using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Enemies;

namespace ArenaSurvivor.Weapons.Targeting
{
    /// <summary>
    /// Abstracción del patrón Strategy para "a quién le apunta un arma". Antes esta búsqueda
    /// estaba copiada en ProjectileWeapon, LaserWeapon, DroneWeapon, DroneMovement y
    /// ChainLightningWeapon (cinco copias casi iguales de "buscar el enemigo más cercano").
    /// Ahora las armas dependen solo de esta interfaz: cambiar a quién disparan es cambiar
    /// el asset de estrategia, no el código del arma (Open/Closed + Dependency Inversion).
    /// </summary>
    public interface ITargetingStrategy
    {
        /// <param name="origin">Desde dónde se mide el alcance.</param>
        /// <param name="range">Alcance máximo.</param>
        /// <param name="excluded">Enemigos que no se pueden elegir (null = ninguno). Lo usa la cadena eléctrica.</param>
        /// <returns>El enemigo elegido, o null si no hay ninguno dentro del alcance.</returns>
        Enemy FindTarget(Vector2 origin, float range, IReadOnlyList<Enemy> excluded);
    }
}
