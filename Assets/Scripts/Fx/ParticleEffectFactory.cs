using UnityEngine;
using ArenaSurvivor.Core.Pooling;

namespace ArenaSurvivor.Fx
{
    //Crea/recicla ráfagas de partículas vía ObjectPool
    public class ParticleEffectFactory
    {
        private readonly ObjectPool<PooledParticleEffect> _pool;

        public ParticleEffectFactory(PooledParticleEffect prefab, Transform poolParent, int prewarmCount = 10)
        {
            _pool = new ObjectPool<PooledParticleEffect>(prefab, poolParent, prewarmCount);
        }

        public void Spawn(Vector2 position, Color color)
        {
            var effect = _pool.Get(position, Quaternion.identity);
            effect.Emit(color, Release);
        }

        private void Release(PooledParticleEffect effect)
        {
            _pool.Release(effect);
        }
    }
}
