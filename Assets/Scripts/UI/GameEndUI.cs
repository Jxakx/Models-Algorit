using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Localization;
using ArenaSurvivor.Managers;

namespace ArenaSurvivor.UI
{
    /// <summary>
    /// Panel de fin de partida por VICTORIA — la derrota tiene su propio tratamiento (el
    /// pantallazo azul, ver BsodUI), así que este solo escucha GameWonEvent.
    /// Con el panel activo, R reinicia la escena.
    /// </summary>
    public class GameEndUI : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Text messageText;

        private bool _isShowingPanel;

        private void Awake()
        {
            panelRoot.SetActive(false);
        }

        private void OnEnable()
        {
            EventManager.Subscribe<GameWonEvent>(OnGameWon);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<GameWonEvent>(OnGameWon);
        }

        private void Update()
        {
            if (_isShowingPanel && Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        private void OnGameWon(GameWonEvent gameEvent)
        {
            messageText.text = Localizer.Get(LocKeys.EndWin);
            ShowPanel();
        }

        private void ShowPanel()
        {
            _isShowingPanel = true;
            panelRoot.SetActive(true);
        }
    }
}
