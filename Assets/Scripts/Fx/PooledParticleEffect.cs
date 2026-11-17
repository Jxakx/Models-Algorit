using System;
using UnityEngine;
using ArenaSurvivor.Core.Pooling;

namespace ArenaSurvivor.Fx
{
    /// <summary>
    /// Envoltorio pooleable de un ParticleSystem: lo reproduce una vez y se libera solo al
    /// pool cuando termina — mismo esquema que Projectile/XpGem (Object Pool).
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledParticleEffect : MonoBehaviour, IPoolable
    {
        private ParticleSystem _particles;
        private Action<PooledParticleEffect> _releaseToPool;
        private float _timer;
        private float _duration;

        private void Awake()
        {
            _particles = GetComponent<ParticleSystem>();
        }

        /// <summary>Tinta y dispara el efecto. Se llama justo después de sacarlo del pool.</summary>
        public void Emit(Color color, Action<PooledParticleEffect> releaseToPool)
        {
            _releaseToPool = releaseToPool;

            var main = _particles.main;
            main.startColor = color;
            _duration = main.duration + main.startLifetime.constantMax;

            _particles.Clear();
            _particles.Play();
        }

        public void OnSpawn()
        {
            _timer = 0f;
        }

        public void OnDespawn()
        {
            _particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _duration)
            {
                _releaseToPool?.Invoke(this);
            }
        }
    }
}
