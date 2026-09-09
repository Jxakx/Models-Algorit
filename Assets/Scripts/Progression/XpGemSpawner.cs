using UnityEngine;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Enemies;

namespace ArenaSurvivor.Progression
{
    /// <summary>Al morir un enemigo (evento), aparece una gema de XP en su posición.</summary>
    public class XpGemSpawner : MonoBehaviour
    {
        [SerializeField] private XpGem gemPrefab;
        [SerializeField] private Transform player;

        private XpGemFactory _factory;

        private void Awake()
        {
            _factory = new XpGemFactory(gemPrefab, transform);
        }

        private void OnEnable()
        {
            EventManager.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void OnEnemyKilled(EnemyKilledEvent gameEvent)
        {
            _factory.Create(gameEvent.Position, gameEvent.ExperienceReward, player);
        }
    }
}
