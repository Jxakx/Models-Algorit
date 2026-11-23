namespace ArenaSurvivor.Enemies.Movement
{
    public interface IMovementStrategy
    {
        //Velocidad que debe tener el enemigo ahora
        UnityEngine.Vector2 CalculateVelocity(MovementContext context);
    }
}
