using System;
using UnityEngine;
using ArenaSurvivor.Core.Audio;

namespace ArenaSurvivor.Weapons
{
    [Serializable]
    public struct WeaponLevelStats
    {
        public float Damage;
        public float Cooldown;
        public int ProjectileCount;
    }

    /// <summary>
    /// Configuración de un arma y cómo escala con el nivel
    /// Look-Up Table: el rendimiento por nivel (daño, cooldown, cantidad de proyectiles) vive en una TABLA indexada por nivel
    /// También es un FLYWEIGHT: lo comparten todos los proyectiles que dispara el arma.
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Arena Survivor/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] private string weaponName = "Arma";

        [Tooltip("Clave de localización del nombre del arma (ver LocalizationDefaults). Si está vacía se usa 'Weapon Name'.")]
        [SerializeField] private string nameKey;

        [SerializeField] private int maxLevel = 5;

        [Tooltip("Sonido que se pide a la SoundBank cada vez que el arma dispara.")]
        [SerializeField] private SoundId fireSound = SoundId.Shoot;

        [Header("Generador de la tabla (solo para 'Regenerar tabla')")]
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float damagePerLevel = 4f;
        [SerializeField] private float baseCooldown = 1f;
        [SerializeField] private float cooldownReductionPerLevel = 0.08f;
        [SerializeField] private float minCooldown = 0.25f;

        [Header("Proyectil")]
        [SerializeField] private float projectileSpeed = 12f;
        [SerializeField] private float projectileLifetime = 3f;

        [Header("Look-Up Table por nivel (índice 0 = nivel 1)")]
        [SerializeField] private WeaponLevelStats[] levelTable;

        public string WeaponName => weaponName;
        public string NameKey => string.IsNullOrEmpty(nameKey) ? weaponName : nameKey;
        public int MaxLevel => maxLevel;
        public SoundId FireSound => fireSound;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileLifetime => projectileLifetime;

        public float GetDamage(int level) => GetStats(level).Damage;

        public float GetCooldown(int level) => GetStats(level).Cooldown;

        public int GetProjectileCount(int level) => GetStats(level).ProjectileCount;

        public WeaponLevelStats GetStats(int level)
        {
            EnsureTable();
            return levelTable[Mathf.Clamp(level, 1, levelTable.Length) - 1];
        }

        [ContextMenu("Regenerar tabla desde los parámetros base")]
        public void RebuildTable()
        {
            var levels = Mathf.Max(1, maxLevel);
            levelTable = new WeaponLevelStats[levels];

            for (var i = 0; i < levels; i++)
            {
                levelTable[i] = new WeaponLevelStats
                {
                    Damage = baseDamage + damagePerLevel * i,
                    Cooldown = Mathf.Max(minCooldown, baseCooldown - cooldownReductionPerLevel * i),
                    ProjectileCount = 1 + i / 2
                };
            }
        }

        private void EnsureTable()
        {
            if (levelTable == null || levelTable.Length != Mathf.Max(1, maxLevel))
            {
                RebuildTable();
            }
        }
    }
}
