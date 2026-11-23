using UnityEngine;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Enemies
{
    //Se publica cuando un enemigo muere: quién escucha decide qué hacer
    public readonly struct EnemyKilledEvent : IGameEvent
    {
        public readonly Vector2 Position;
        public readonly int ExperienceReward;
        public readonly Color Color;

        public EnemyKilledEvent(Vector2 position, int experienceReward, Color color)
        {
            Position = position;
            ExperienceReward = experienceReward;
            Color = color;
        }
    }
}
