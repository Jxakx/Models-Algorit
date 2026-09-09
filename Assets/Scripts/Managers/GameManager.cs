using UnityEngine;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Enemies;
using ArenaSurvivor.Player;

namespace ArenaSurvivor.Managers
{
    /// <summary>
    /// Estado general de la partida: cronómetro de supervivencia, contador de bajas, y
    /// condición de victoria (sobrevivir winTimeSeconds) / derrota (el jugador muere).
    /// No conoce a la UI directamente — todo pasa por el EventManager (Observer).
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float winTimeSeconds = 180f;

        private float _elapsed;
        private int _killCount;
        private bool _gameEnded;

        private void OnEnable()
        {
            EventManager.Subscribe<PlayerDiedEvent>(OnPlayerDied);
            EventManager.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
            EventManager.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void Update()
        {
            if (_gameEnded)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            EventManager.Publish(new GameTimeChangedEvent(_elapsed));

            if (_elapsed >= winTimeSeconds)
            {
                _gameEnded = true;
                Time.timeScale = 0f;
                EventManager.Publish(new GameWonEvent());
            }
        }

        private void OnEnemyKilled(EnemyKilledEvent gameEvent)
        {
            _killCount++;
            EventManager.Publish(new KillCountChangedEvent(_killCount));
        }

        private void OnPlayerDied(PlayerDiedEvent gameEvent)
        {
            if (_gameEnded)
            {
                return;
            }

            _gameEnded = true;
            Time.timeScale = 0f;
            EventManager.Publish(new GameOverEvent());
        }
    }
}
