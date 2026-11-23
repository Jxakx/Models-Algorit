using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaSurvivor.Core.Flyweight
{
    public static class FlyweightFactory<T> where T : ScriptableObject
    {
        private static readonly Dictionary<string, T> ByKey = new Dictionary<string, T>();
        private static readonly Dictionary<T, int> UsersByFlyweight = new Dictionary<T, int>();
        private static int _contextCount;

        //Cuantos flyweights distintos se usaron hasta ahora
        public static int SharedCount => UsersByFlyweight.Count;

        //Cuantos objetos están usando un flyweight en este momento
        public static int ContextCount => _contextCount;

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

        //Un contexto empieza a usar este flyweight. Devuelve el mismo objeto compartido
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

        //Un contexto deja de usar este flyweight
        public static void Release(T flyweight)
        {
            if (flyweight == null || !UsersByFlyweight.TryGetValue(flyweight, out var users) || users <= 0)
            {
                return;
            }

            UsersByFlyweight[flyweight] = users - 1;
            _contextCount--;
        }

        public static void ResetUsage()
        {
            UsersByFlyweight.Clear();
            _contextCount = 0;
        }
    }
}
