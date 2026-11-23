using UnityEngine;

namespace ArenaSurvivor.Fx
{
    /// <summary>
    /// FLYWEIGHT de los números de daño flotantes: el aspecto y el comportamiento que es igual para todos los números del mismo "tipo" 
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
