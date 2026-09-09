using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Managers;

namespace ArenaSurvivor.UI
{
    /// <summary>Muestra el tiempo sobrevivido en formato MM:SS. Solo escucha GameTimeChangedEvent.</summary>
    public class GameTimerUI : MonoBehaviour
    {
        [SerializeField] private Text timerText;

        private void OnEnable()
        {
            EventManager.Subscribe<GameTimeChangedEvent>(OnTimeChanged);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<GameTimeChangedEvent>(OnTimeChanged);
        }

        private void OnTimeChanged(GameTimeChangedEvent gameEvent)
        {
            var totalSeconds = Mathf.FloorToInt(gameEvent.ElapsedSeconds);
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
