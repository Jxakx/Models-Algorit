using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaSurvivor.Core.Flyweight
{
    /// <summary>
    /// Fábrica de flyweights (patrón Flyweight). Hay UNA por cada tipo de dato compartido
    /// (FlyweightFactory&lt;EnemyData&gt;, FlyweightFactory&lt;WeaponData&gt;, ...) y cumple los dos
    /// roles que le pide el patrón:
    ///
    /// 1) Garantiza que exista una única instancia compartida por clave: GetOrCreate crea el
    ///    flyweight la primera vez que alguien lo pide y a partir de ahí todos reciben la
    ///    misma (por ejemplo, el movimiento "perseguir" por defecto: se crea una vez y lo
    ///    usan todos los enemigos que no tienen uno asignado).
    /// 2) Lleva la cuenta de cuántos contextos (objetos livianos como un Enemy o un
    ///    Projectile) están usando hoy cada flyweight — Acquire al tomarlo, Release al
    ///    soltarlo. Eso permite MOSTRAR el ahorro: "87 enemigos vivos, 3 datos en memoria".
    ///
    /// Reparto de estado, que es la idea central del patrón:
    ///  - INTRÍNSECO (compartido, inmutable): vida máxima, velocidad, color, estilo... vive
    ///    en el flyweight (un ScriptableObject) y existe una sola vez, no una por enemigo.
    ///  - EXTRÍNSECO (propio de cada instancia): posición, vida actual, objetivo, cooldown...
    ///    vive en el contexto, que solo guarda una referencia al flyweight.
    /// </summary>
    public static class FlyweightFactory<T> where T : ScriptableObject
    {
        private static readonly Dictionary<string, T> ByKey = new Dictionary<string, T>();
        private static readonly Dictionary<T, int> UsersByFlyweight = new Dictionary<T, int>();
        private static int _contextCount;

        /// <summary>Cuántos flyweights distintos se usaron hasta ahora.</summary>
        public static int SharedCount => UsersByFlyweight.Count;

        /// <summary>Cuántos objetos (contextos) están usando un flyweight en este momento.</summary>
        public static int ContextCount => _contextCount;

        /// <summary>
        /// Devuelve el flyweight guardado bajo esa clave; si no existe (o Unity ya lo destruyó),
        /// lo crea con <paramref name="create"/> y lo guarda para los próximos pedidos.
        /// </summary>
        public static T GetOrCreate(string key, Func<T> create)
        {
            if (ByKey.TryGetValue(key, out var existing) && existing != null)
            {
                return existing;
            }

            var created = create();
            ByKey[key] = created;
            return created;
        }

        /// <summary>Un contexto empieza a usar este flyweight. Devuelve el mismo objeto compartido.</summary>
        public static T Acquire(T flyweight)
        {
            if (flyweight == null)
            {
                return null;
            }

            UsersByFlyweight.TryGetValue(flyweight, out var users);
            UsersByFlyweight[flyweight] = users + 1;
            _contextCount++;
            return flyweight;
        }

        /// <summary>Un contexto deja de usar este flyweight (se recicló o se destruyó).</summary>
        public static void Release(T flyweight)
        {
            if (flyweight == null || !UsersByFlyweight.TryGetValue(flyweight, out var users) || users <= 0)
            {
                return;
            }

            UsersByFlyweight[flyweight] = users - 1;
            _contextCount--;
        }

        /// <summary>
        /// Pone en cero los contadores de uso (no los flyweights compartidos). Se llama al
        /// arrancar una escena: los contextos de la escena anterior se destruyeron sin pasar
        /// por Release, y sin esto el conteo arrastraría objetos que ya no existen.
        /// </summary>
        public static void ResetUsage()
        {
            UsersByFlyweight.Clear();
            _contextCount = 0;
        }
    }
}
