using UnityEngine;
using ArenaSurvivor.Core.Flyweight;
using ArenaSurvivor.Enemies.Movement;

namespace ArenaSurvivor.Enemies
{
    /// <summary>
    /// Configuración de un "tipo" de enemigo. Agregar un tipo nuevo es
    /// crear un asset de este ScriptableObject, no escribir una clase nueva así el
    /// EnemyFactory no necesita cambiar cuando el juego crece.
    ///
    /// Es el FLYWEIGHT de los enemigos: guarda el estado intrínseco (el que es igual para
    /// todos los enemigos del mismo tipo) una sola vez, de solo lectura. Los 150 enemigos
    /// vivos de un nivel no copian estos números: cada Enemy guarda únicamente una
    /// referencia a su EnemyData y su propio estado.
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

        [Header("Comportamiento (Strategy)")]
        [Tooltip("Cómo se mueve este tipo de enemigo. Vacío = perseguir en línea recta.")]
        [SerializeField] private MovementStrategy movement;

        public float MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public float ContactDamage => contactDamage;
        public int ExperienceReward => experienceReward;
        public Color Color => color;
        public float Scale => scale;

        /// <summary>
        /// La estrategia de movimiento de este tipo. Si no se asignó ninguna, usa una
        /// "perseguir" por defecto que se crea una sola vez y se comparte entre todos los
        /// tipos sin estrategia.
        /// </summary>
        public IMovementStrategy Movement => movement != null ? movement : DefaultMovement();

        private static IMovementStrategy DefaultMovement()
        {
            return FlyweightFactory<MovementStrategy>.GetOrCreate("default-chase", () =>
            {
                var chase = CreateInstance<ChaseMovement>();
                chase.hideFlags = HideFlags.HideAndDontSave;
                return chase;
            });
        }
    }
}
