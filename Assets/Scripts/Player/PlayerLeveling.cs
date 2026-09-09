using UnityEngine;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Player
{
    /// <summary>
    /// Acumula XP recolectada (evento global) y sube de nivel cuando corresponde.
    /// No sabe nada de UI ni de mejoras — solo publica los eventos correspondientes
    /// y que cada quien (HUD, LevelUpManager) reaccione por su cuenta (Observer).
    /// </summary>
    public class PlayerLeveling : MonoBehaviour
    {
        [SerializeField] private int baseXpToLevel = 25;
        [SerializeField] private float xpCurveMultiplier = 1.35f;

        public int Level { get; private set; } = 1;
        public int CurrentXp { get; private set; }
        public int XpToNextLevel { get; private set; }

        private void Awake()
        {
            XpToNextLevel = baseXpToLevel;
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
                XpToNextLevel = Mathf.RoundToInt(XpToNextLevel * xpCurveMultiplier);
                EventManager.Publish(new PlayerLeveledUpEvent(Level));
            }

            PublishXpChanged();
        }

        private void PublishXpChanged()
        {
            EventManager.Publish(new PlayerXpChangedEvent(CurrentXp, XpToNextLevel, Level));
        }
    }
}
