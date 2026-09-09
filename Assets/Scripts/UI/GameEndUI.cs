using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Managers;

namespace ArenaSurvivor.UI
{
    /// <summary>
    /// Panel de fin de partida (derrota o victoria, mismo panel con distinto texto).
    /// Escucha GameOverEvent/GameWonEvent; con el panel activo, R reinicia la escena.
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
            EventManager.Subscribe<GameOverEvent>(OnGameOver);
            EventManager.Subscribe<GameWonEvent>(OnGameWon);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<GameOverEvent>(OnGameOver);
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

        private void OnGameOver(GameOverEvent gameEvent)
        {
            messageText.text = "GAME OVER\n\nPresioná R para reintentar";
            ShowPanel();
        }

        private void OnGameWon(GameWonEvent gameEvent)
        {
            messageText.text = "¡GANASTE!\n\nPresioná R para reintentar";
            ShowPanel();
        }

        private void ShowPanel()
        {
            _isShowingPanel = true;
            panelRoot.SetActive(true);
        }
    }
}
