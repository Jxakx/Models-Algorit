using UnityEngine;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Player
{
    /// <summary>
    /// Acumula XP recolectada (evento global) y sube de nivel cuando corresponde.
    /// No sabe nada de UI ni de mejoras — solo publica los eventos correspondientes
    /// y que cada quien (HUD, LevelUpManager) reaccione por su cuenta (Observer).
    ///
    /// Look-Up Table: cuánta XP pide cada nivel sale de una TABLA indexada por nivel (editable
    /// en el Inspector), no de multiplicar en cada subida de nivel. Los parámetros base solo
    /// sirven para llenarla (menú contextual "Regenerar tabla de XP"); pasado el último nivel
    /// de la tabla, la curva sigue creciendo con el mismo multiplicador.
    /// </summary>
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

        /// <summary>XP que hace falta para pasar del nivel indicado al siguiente (un acceso a la tabla).</summary>
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

            // Más allá de la tabla: sigue la misma curva a partir de la última fila.
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
