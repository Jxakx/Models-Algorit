using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Managers;

namespace ArenaSurvivor.UI
{
    /// <summary>Muestra la cantidad de enemigos eliminados. Solo escucha KillCountChangedEvent.</summary>
    public class KillCounterUI : MonoBehaviour
    {
        [SerializeField] private Text countText;

        private void OnEnable()
        {
            EventManager.Subscribe<KillCountChangedEvent>(OnKillCountChanged);
            countText.text = "Kills: 0";
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<KillCountChangedEvent>(OnKillCountChanged);
        }

        private void OnKillCountChanged(KillCountChangedEvent gameEvent)
        {
            countText.text = $"Kills: {gameEvent.Count}";
        }
    }
}
