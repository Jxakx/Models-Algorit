using UnityEngine;

namespace ArenaSurvivor.Enemies
{
    /// <summary>
    /// Configuración de un "tipo" de enemigo (swarm, tank, etc.). Agregar un tipo nuevo es
    /// crear un asset de este ScriptableObject, no escribir una clase nueva — así el
    /// EnemyFactory no necesita cambiar cuando el juego crece (Open/Closed).
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Arena Survivor/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [SerializeField] private float maxHealth = 20f;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float contactDamage = 10f;
        [SerializeField] private int experienceReward = 5;
        [SerializeField] private Color color = Color.red;
        [SerializeField] private float scale = 1f;

        public float MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public float ContactDamage => contactDamage;
        public int ExperienceReward => experienceReward;
        public Color Color => color;
        public float Scale => scale;
    }
}
