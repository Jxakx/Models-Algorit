using UnityEngine;

namespace ArenaSurvivor.Fx
{
    /// <summary>
    /// FLYWEIGHT de los números de daño flotantes: el aspecto y el comportamiento que es
    /// igual para todos los números del mismo "tipo" (color, velocidad de subida, duración).
    /// En pantalla puede haber decenas de números a la vez, pero solo existen dos estilos
    /// (daño a enemigos y daño al jugador): cada FloatingDamageNumber guarda únicamente su
    /// estado extrínseco (monto, posición, cronómetro) y una referencia a su estilo.
    /// </summary>
    [CreateAssetMenu(fileName = "DamageStyle", menuName = "Arena Survivor/Damage Number Style")]
    public class DamageNumberStyle : ScriptableObject
    {
        [SerializeField] private Color color = Color.white;
        [SerializeField] private float moveSpeed = 1.2f;
        [SerializeField] private float lifetime = 0.6f;

        public Color Color => color;
        public float MoveSpeed => moveSpeed;
        public float Lifetime => lifetime;
    }
}
