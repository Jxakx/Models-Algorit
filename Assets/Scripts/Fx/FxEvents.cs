using UnityEngine;
using ArenaSurvivor.Core.Audio;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Fx
{
    /// <summary>
    /// Se publica cada vez que un arma dispara. Lleva el
    /// SoundId con el que dispara esa arma para que quien escucha
    /// reproduzca el sonido correcto sin saber qué arma fue
    /// </summary>
    public readonly struct WeaponFiredEvent : IGameEvent
    {
        public readonly SoundId Sound;

        public WeaponFiredEvent(SoundId sound)
        {
            Sound = sound;
        }
    }

    //Se publica cada vez que un arma conecta un golpe dispara chispas de impacto y sonido
    public readonly struct WeaponImpactEvent : IGameEvent
    {
        public readonly Vector2 Position;

        public WeaponImpactEvent(Vector2 position)
        {
            Position = position;
        }
    }
    /// Se publica cada vez que alguien recibe daño, para mostrar el número flotante correspondiente
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
