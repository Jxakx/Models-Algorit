using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Core.Flyweight;
using ArenaSurvivor.Enemies;
using ArenaSurvivor.Weapons.Targeting;

namespace ArenaSurvivor.Weapons
{
    /// <summary>
    /// Arma que necesita elegir a quién apuntar. Delega esa decisión en una estrategia de
    /// targeting (patrón Strategy) asignada desde el Inspector, en vez de tener su propia
    /// búsqueda escrita adentro. OrbitalWeapon no hereda de acá porque no apunta a nadie:
    /// gira alrededor del jugador (por eso esto no está en WeaponBase — Interface Segregation).
    /// </summary>
    public abstract class TargetedWeapon : WeaponBase
    {
        [Header("Targeting (Strategy)")]
        [Tooltip("A quién le apunta esta arma. Vacío = el más cercano.")]
        [SerializeField] private TargetingStrategy targetingStrategy;

        /// <summary>Elige un objetivo con la estrategia configurada (o la del más cercano si no hay ninguna).</summary>
        public Enemy FindTarget(Vector2 origin, float range, IReadOnlyList<Enemy> excluded = null)
        {
            return CurrentStrategy().FindTarget(origin, range, excluded);
        }

        private ITargetingStrategy CurrentStrategy()
        {
            return targetingStrategy != null ? targetingStrategy : DefaultStrategy();
        }

        // Estrategia por defecto compartida: se crea una sola vez y la usan todas las armas
        // sin estrategia asignada (la entrega la FlyweightFactory).
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
