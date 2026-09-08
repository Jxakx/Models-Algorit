using System;
using UnityEngine;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Core.Pooling;
using ArenaSurvivor.Player;

namespace ArenaSurvivor.Progression
{
    /// <summary>
    /// Gema de XP pooleada: si el jugador entra en su radio de imán, vuela hacia él; al
    /// tocarlo, publica XpCollectedEvent y se libera al pool. No conoce a PlayerLeveling
    /// directamente — solo publica el evento (Observer).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class XpGem : MonoBehaviour, IPoolable
    {
        [SerializeField] private float magnetRadius = 5f;
        [SerializeField] private float magnetSpeed = 12f;

        private Rigidbody2D _rigidbody;
        private Transform _player;
        private Action<XpGem> _releaseToPool;
        private int _xpValue;
        private bool _released;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Initialize(Vector2 position, int xpValue, Transform player, Action<XpGem> releaseToPool)
        {
            transform.position = position;
            _xpValue = xpValue;
            _player = player;
            _releaseToPool = releaseToPool;
        }

        public void OnSpawn()
        {
            _released = false;
        }

        public void OnDespawn()
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }

        private void FixedUpdate()
        {
            if (_player == null)
            {
                return;
            }

            var toPlayer = (Vector2)_player.position - _rigidbody.position;
            if (toPlayer.magnitude <= magnetRadius)
            {
                _rigidbody.linearVelocity = toPlayer.normalized * magnetSpeed;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_released || !other.CompareTag("Player"))
            {
                return;
            }

            _released = true;
            EventManager.Publish(new XpCollectedEvent(_xpValue));
            _releaseToPool?.Invoke(this);
        }
    }
}
