using System;
using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Managers;
using ArenaSurvivor.Player;

namespace ArenaSurvivor.Enemies
{
    /// <summary>
    /// Un tipo de enemigo disponible para el spawner, y a partir de qué momento
    /// (en segundos desde que arrancó la partida) empieza a poder aparecer.
    /// </summary>
    [Serializable]
    public class EnemyWaveEntry
    {
        public EnemyData Data;
        public float UnlockTimeSeconds;
    }

    /// <summary>
    /// Genera enemigos alrededor del jugador, fuera de cámara, con una tasa que aumenta
    /// con el tiempo (más rápido = más difícil) y mezclando tipos más fuertes a medida que
    /// se desbloquean. Usa el EnemyFactory (Pool + Factory) para no instanciar/destruir nunca.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private Transform target;
        [SerializeField] private EnemyWaveEntry[] waveEntries;

        [SerializeField] private float initialSpawnInterval = 2f;
        [SerializeField] private float minSpawnInterval = 0.35f;
        [SerializeField] private float spawnIntervalRampSeconds = 180f;

        [SerializeField] private float spawnRadiusMin = 18f;
        [SerializeField] private float spawnRadiusMax = 24f;
        [SerializeField] private int maxAliveEnemies = 150;

        [Tooltip("Transform del Ground (un Square escalado): sus límites se calculan a partir " +
                 "de su escala, así que un spawn nunca cae afuera del área jugable.")]
        [SerializeField] private Transform groundTransform;
        [SerializeField] private float groundEdgeMargin = 2f;

        private EnemyFactory _factory;
        private readonly List<EnemyData> _unlockedBuffer = new List<EnemyData>();

        private float _elapsed;
        private float _spawnTimer;
        private bool _isSpawning = true;
        private int _aliveCount;

        private void Awake()
        {
            _factory = new EnemyFactory(enemyPrefab, transform, prewarmCount: 40);
        }

        private void OnEnable()
        {
            EventManager.Subscribe<PlayerDiedEvent>(OnGameEnded);
            EventManager.Subscribe<GameWonEvent>(OnGameEnded);
            EventManager.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<PlayerDiedEvent>(OnGameEnded);
            EventManager.Unsubscribe<GameWonEvent>(OnGameEnded);
            EventManager.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void Update()
        {
            if (!_isSpawning)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer <= 0f)
            {
                _spawnTimer = CurrentSpawnInterval();
                TrySpawnOne();
            }
        }

        private float CurrentSpawnInterval()
        {
            var t = Mathf.Clamp01(_elapsed / spawnIntervalRampSeconds);
            return Mathf.Lerp(initialSpawnInterval, minSpawnInterval, t);
        }

        private void TrySpawnOne()
        {
            if (_aliveCount >= maxAliveEnemies)
            {
                return;
            }

            var data = PickEnemyData();
            if (data == null)
            {
                return;
            }

            var position = GetSpawnPositionAroundTarget();
            _factory.Create(data, position, target);
            _aliveCount++;
        }

        private EnemyData PickEnemyData()
        {
            _unlockedBuffer.Clear();

            foreach (var entry in waveEntries)
            {
                if (entry.Data != null && _elapsed >= entry.UnlockTimeSeconds)
                {
                    _unlockedBuffer.Add(entry.Data);
                }
            }

            if (_unlockedBuffer.Count == 0)
            {
                return null;
            }

            return _unlockedBuffer[UnityEngine.Random.Range(0, _unlockedBuffer.Count)];
        }

        private Vector2 GetSpawnPositionAroundTarget()
        {
            var angle = UnityEngine.Random.value * Mathf.PI * 2f;
            var radius = UnityEngine.Random.Range(spawnRadiusMin, spawnRadiusMax);
            var offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            var position = (Vector2)target.position + offset;

            return ClampToGround(position);
        }

        private Vector2 ClampToGround(Vector2 position)
        {
            if (groundTransform == null)
            {
                return position;
            }

            var halfSize = (Vector2)groundTransform.lossyScale * 0.5f - Vector2.one * groundEdgeMargin;
            var center = (Vector2)groundTransform.position;

            position.x = Mathf.Clamp(position.x, center.x - halfSize.x, center.x + halfSize.x);
            position.y = Mathf.Clamp(position.y, center.y - halfSize.y, center.y + halfSize.y);
            return position;
        }

        private void OnEnemyKilled(EnemyKilledEvent gameEvent)
        {
            _aliveCount = Mathf.Max(0, _aliveCount - 1);
        }

        private void OnGameEnded(PlayerDiedEvent gameEvent) => _isSpawning = false;

        private void OnGameEnded(GameWonEvent gameEvent) => _isSpawning = false;
    }
}
