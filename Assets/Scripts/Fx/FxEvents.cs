using UnityEngine;
using ArenaSurvivor.Core.Audio;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Fx
{
    /// <summary>
    /// Se publica cada vez que un arma dispara (una vez por ráfaga, no por bala). Lleva el
    /// SoundId con el que dispara esa arma (viene de su WeaponData) para que quien escucha
    /// reproduzca el sonido correcto sin saber qué arma fue.
    /// </summary>
    public readonly struct WeaponFiredEvent : IGameEvent
    {
        public readonly SoundId Sound;

        public WeaponFiredEvent(SoundId sound)
        {
            Sound = sound;
        }
    }

    /// <summary>Se publica cada vez que un arma conecta un golpe — dispara chispas de impacto y sonido.</summary>
    public readonly struct WeaponImpactEvent : IGameEvent
    {
        public readonly Vector2 Position;

        public WeaponImpactEvent(Vector2 position)
        {
            Position = position;
        }
    }

    /// <summary>
    /// Se publica cada vez que alguien (jugador o enemigo) recibe daño, para mostrar el
    /// número flotante correspondiente. Lo publican los DamageNumberSpawner locales — no
    /// Health, que se mantiene genérico y ajeno al EventManager (ver Health.cs).
    /// El estilo del número (color, velocidad, duración) viaja como flyweight compartido.
    /// </summary>
    public readonly struct FloatingDamageTextEvent : IGameEvent
    {
        public readonly Vector2 Position;
        public readonly float Amount;
        public readonly DamageNumberStyle Style;

        public FloatingDamageTextEvent(Vector2 position, float amount, DamageNumberStyle style)
        {
            Position = position;
            Amount = amount;
            Style = style;
        }
    }
}
