using UnityEngine;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Enemies
{
    /// <summary>Se publica cuando un enemigo muere: quién escucha decide qué hacer (XP, score, partículas, etc.).</summary>
    public readonly struct EnemyKilledEvent : IGameEvent
    {
        public readonly Vector2 Position;
        public readonly int ExperienceReward;

        /// <summary>Color del tipo de enemigo que murió: las partículas de muerte se tiñen con él.</summary>
        public readonly Color Color;

        public EnemyKilledEvent(Vector2 position, int experienceReward, Color color)
        {
            Position = position;
            ExperienceReward = experienceReward;
            Color = color;
        }
    }
}
