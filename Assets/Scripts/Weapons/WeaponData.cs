using UnityEngine;

namespace ArenaSurvivor.Weapons
{
    /// <summary>
    /// Configuración de un arma y cómo escala con el nivel. Un arma nueva es un asset de
    /// este ScriptableObject (+ el componente concreto que sabe cómo dispararla), no una
    /// clase entera reescrita desde cero.
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Arena Survivor/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] private string weaponName = "Arma";
        [SerializeField] private int maxLevel = 5;
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float damagePerLevel = 4f;
        [SerializeField] private float baseCooldown = 1f;
        [SerializeField] private float cooldownReductionPerLevel = 0.08f;
        [SerializeField] private float minCooldown = 0.25f;
        [SerializeField] private float projectileSpeed = 12f;
        [SerializeField] private float projectileLifetime = 3f;

        public string WeaponName => weaponName;
        public int MaxLevel => maxLevel;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileLifetime => projectileLifetime;

        public float GetDamage(int level) => baseDamage + damagePerLevel * (level - 1);

        public float GetCooldown(int level) =>
            Mathf.Max(minCooldown, baseCooldown - cooldownReductionPerLevel * (level - 1));

        public int GetProjectileCount(int level) => 1 + (level - 1) / 2;
    }
}
