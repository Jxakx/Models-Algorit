namespace ArenaSurvivor.Enemies.Movement
{
    /// <summary>
    /// Abstracción del patrón Strategy para "cómo se mueve un enemigo". Enemy depende de
    /// esta interfaz y no de ninguna forma concreta de moverse (Dependency Inversion):
    /// agregar un comportamiento nuevo es crear una estrategia nueva, sin tocar Enemy
    /// (Open/Closed).
    /// </summary>
    public interface IMovementStrategy
    {
        /// <summary>Velocidad (dirección y magnitud) que debe tener el enemigo ahora.</summary>
        UnityEngine.Vector2 CalculateVelocity(MovementContext context);
    }
}
