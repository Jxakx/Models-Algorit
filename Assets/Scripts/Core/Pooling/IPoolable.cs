namespace ArenaSurvivor.Core.Pooling
{
    /// <summary>
    /// Implementado por componentes que necesitan resetear su propio estado cuando el
    /// ObjectPool los entrega (OnSpawn) o los recicla (OnDespawn) — por ejemplo, restaurar
    /// vida, limpiar velocidad, o reiniciar un timer. El pool no necesita saber qué resetea
    /// cada tipo de objeto: cada uno se encarga de sí mismo.
    /// </summary>
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
    }
}
