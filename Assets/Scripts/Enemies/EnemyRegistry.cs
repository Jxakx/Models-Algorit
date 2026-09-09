using System.Collections.Generic;

namespace ArenaSurvivor.Enemies
{
    /// <summary>
    /// Lista de enemigos activos en este momento. Cada Enemy se registra/desregistra solo
    /// (OnEnable/OnDisable), lo que funciona perfecto con el pooling: un enemigo reciclado
    /// se desactiva (sale de la lista) y al reusarse se reactiva (vuelve a entrar).
    /// Las armas la consultan para encontrar el enemigo más cercano sin acoplarse al Spawner.
    /// </summary>
    public static class EnemyRegistry
    {
        private static readonly List<Enemy> Active = new List<Enemy>();

        public static IReadOnlyList<Enemy> ActiveEnemies => Active;

        public static void Register(Enemy enemy)
        {
            if (!Active.Contains(enemy))
            {
                Active.Add(enemy);
            }
        }

        public static void Unregister(Enemy enemy)
        {
            Active.Remove(enemy);
        }
    }
}
