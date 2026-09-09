using UnityEngine;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Enemies
{
    /// <summary>Se publica cuando un enemigo muere: quién escucha decide qué hacer (XP, score, etc.).</summary>
    public readonly struct EnemyKilledEvent : IGameEvent
    {
        public readonly Vector2 Position;
        public readonly int ExperienceReward;

        public EnemyKilledEvent(Vector2 position, int experienceReward)
        {
            Position = position;
            ExperienceReward = experienceReward;
        }
    }
}
