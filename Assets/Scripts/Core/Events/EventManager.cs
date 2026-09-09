using System;
using System.Collections.Generic;

namespace ArenaSurvivor.Core.Events
{
    /// <summary>
    /// Bus de eventos genérico (patrón Observer): los "sujetos" publican eventos sin conocer
    /// quién los escucha, y los "observadores" se suscriben al tipo de evento que les interesa.
    ///
    /// Agregar un evento nuevo NO requiere modificar esta clase: solo hay que crear un tipo
    /// que implemente IGameEvent y usarlo con Subscribe/Publish (Open/Closed Principle).
    /// </summary>
    public static class EventManager
    {
        private static readonly Dictionary<Type, Delegate> Listeners = new Dictionary<Type, Delegate>();

        public static void Subscribe<T>(Action<T> listener) where T : IGameEvent
        {
            var eventType = typeof(T);

            if (Listeners.TryGetValue(eventType, out var existing))
            {
                Listeners[eventType] = Delegate.Combine(existing, listener);
            }
            else
            {
                Listeners[eventType] = listener;
            }
        }

        public static void Unsubscribe<T>(Action<T> listener) where T : IGameEvent
        {
            var eventType = typeof(T);

            if (!Listeners.TryGetValue(eventType, out var existing))
            {
                return;
            }

            var updated = Delegate.Remove(existing, listener);
            if (updated == null)
            {
                Listeners.Remove(eventType);
            }
            else
            {
                Listeners[eventType] = updated;
            }
        }

        public static void Publish<T>(T gameEvent) where T : IGameEvent
        {
            var eventType = typeof(T);

            if (Listeners.TryGetValue(eventType, out var existing) && existing is Action<T> callback)
            {
                callback.Invoke(gameEvent);
            }
        }

        /// <summary>
        /// Saca a todos los oyentes registrados. Útil al reiniciar la partida (Game Over / Retry)
        /// para no arrastrar suscripciones de objetos que ya no existen.
        /// </summary>
        public static void Clear()
        {
            Listeners.Clear();
        }
    }
}
