using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Localization;
using ArenaSurvivor.Managers;

namespace ArenaSurvivor.UI
{
    /// <summary>
    /// Menú de pausa: Reanudar, Reiniciar y cambio de idioma (ES/EN). Se muestra y se oculta
    /// escuchando GamePausedEvent / GameResumedEvent — no sabe cómo ni cuándo se pausa, y
    /// el PauseController no sabe que existe este panel. Los botones solo le piden las
    /// acciones al controlador (o al servicio de idiomas).
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private PauseController pauseController;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button languageButton;

        private void Awake()
        {
            panelRoot.SetActive(false);

            resumeButton.onClick.AddListener(OnResumeClicked);
            restartButton.onClick.AddListener(OnRestartClicked);
            languageButton.onClick.AddListener(OnLanguageClicked);
        }

        private void OnEnable()
        {
            EventManager.Subscribe<GamePausedEvent>(OnPaused);
            EventManager.Subscribe<GameResumedEvent>(OnResumed);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<GamePausedEvent>(OnPaused);
            EventManager.Unsubscribe<GameResumedEvent>(OnResumed);
        }

        private void OnPaused(GamePausedEvent gameEvent) => panelRoot.SetActive(true);

        private void OnResumed(GameResumedEvent gameEvent) => panelRoot.SetActive(false);

        private void OnResumeClicked()
        {
            EventManager.Publish(new UiButtonClickedEvent());
            pauseController.Resume();
        }

        private void OnRestartClicked()
        {
            EventManager.Publish(new UiButtonClickedEvent());
            pauseController.Restart();
        }

        private void OnLanguageClicked()
        {
            EventManager.Publish(new UiButtonClickedEvent());
            Localizer.ToggleLanguage();
        }
    }
}
