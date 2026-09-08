using UnityEngine;
using ArenaSurvivor.Core.Pooling;

namespace ArenaSurvivor.Progression
{
    /// <summary>Crea/recicla gemas de XP vía ObjectPool — mismo patrón que EnemyFactory.</summary>
    public class XpGemFactory
    {
        private readonly ObjectPool<XpGem> _pool;

        public XpGemFactory(XpGem prefab, Transform poolParent, int prewarmCount = 40)
        {
            _pool = new ObjectPool<XpGem>(prefab, poolParent, prewarmCount);
        }

        public XpGem Create(Vector2 position, int xpValue, Transform player)
        {
            var gem = _pool.Get(position, Quaternion.identity);
            gem.Initialize(position, xpValue, player, Release);
            return gem;
        }

        private void Release(XpGem gem)
        {
            _pool.Release(gem);
        }
    }
}
