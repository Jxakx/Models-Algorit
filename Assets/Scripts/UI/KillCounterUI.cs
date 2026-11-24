using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Localization;
using ArenaSurvivor.Managers;

namespace ArenaSurvivor.UI
{
    // Muestra la cantidad de enemigos eliminados
    public class KillCounterUI : MonoBehaviour
    {
        [SerializeField] private Text countText;

        private int _count;

        private void OnEnable()
        {
            EventManager.Subscribe<KillCountChangedEvent>(OnKillCountChanged);
            EventManager.Subscribe<LanguageChangedEvent>(OnLanguageChanged);
            Redraw();
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<KillCountChangedEvent>(OnKillCountChanged);
            EventManager.Unsubscribe<LanguageChangedEvent>(OnLanguageChanged);
        }

        private void OnKillCountChanged(KillCountChangedEvent gameEvent)
        {
            _count = gameEvent.Count;
            Redraw();
        }

        private void OnLanguageChanged(LanguageChangedEvent gameEvent) => Redraw();

        private void Redraw()
        {
            countText.text = Localizer.Get(LocKeys.HudKills, _count);
        }
    }
}
