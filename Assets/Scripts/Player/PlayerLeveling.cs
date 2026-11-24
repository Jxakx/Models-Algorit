using UnityEngine;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Player
{
    // Acumula XP recolectada (evento global) y sube de nivel cuando corresponde.
    // Look-Up Table: cuánta XP pide cada nivel sale de una TABLA indexada por nivel
    public class PlayerLeveling : MonoBehaviour
    {
        private const int TableLevels = 40;

        [SerializeField] private int baseXpToLevel = 25;
        [SerializeField] private float xpCurveMultiplier = 1.35f;

        [Header("Look-Up Table de XP (índice 0 = del nivel 1 al 2)")]
        [SerializeField] private int[] xpPerLevel;

        public int Level { get; private set; } = 1;
        public int CurrentXp { get; private set; }
        public int XpToNextLevel { get; private set; }

        private void Awake()
        {
            XpToNextLevel = XpForLevel(Level);
        }

        private void OnEnable()
        {
            EventManager.Subscribe<XpCollectedEvent>(OnXpCollected);
            PublishXpChanged();
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<XpCollectedEvent>(OnXpCollected);
        }

        private void OnXpCollected(XpCollectedEvent gameEvent)
        {
            CurrentXp += gameEvent.Amount;

            while (CurrentXp >= XpToNextLevel)
            {
                CurrentXp -= XpToNextLevel;
                Level++;
                XpToNextLevel = XpForLevel(Level);
                EventManager.Publish(new PlayerLeveledUpEvent(Level));
            }

            PublishXpChanged();
        }

        private void PublishXpChanged()
        {
            EventManager.Publish(new PlayerXpChangedEvent(CurrentXp, XpToNextLevel, Level));
        }

        private int XpForLevel(int level)
        {
            if (xpPerLevel == null || xpPerLevel.Length == 0)
            {
                RebuildXpTable();
            }

            var index = level - 1;
            if (index < xpPerLevel.Length)
            {
                return xpPerLevel[index];
            }

            var xp = xpPerLevel[xpPerLevel.Length - 1];
            for (var i = xpPerLevel.Length; i <= index; i++)
            {
                xp = Mathf.RoundToInt(xp * xpCurveMultiplier);
            }

            return xp;
        }

        [ContextMenu("Regenerar tabla de XP desde los parámetros base")]
        public void RebuildXpTable()
        {
            xpPerLevel = new int[TableLevels];
            xpPerLevel[0] = baseXpToLevel;

            for (var i = 1; i < TableLevels; i++)
            {
                xpPerLevel[i] = Mathf.RoundToInt(xpPerLevel[i - 1] * xpCurveMultiplier);
            }
        }
    }
}
