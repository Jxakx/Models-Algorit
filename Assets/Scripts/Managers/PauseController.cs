using UnityEngine;
using UnityEngine.SceneManagement;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Managers
{
    /// <summary>
    /// Pausa/despausa el gameplay con ESC — pisando Time.timeScale, el mismo mecanismo que
    /// ya usan el panel de nivel y el fin de partida. Solo maneja el ESTADO de pausa: avisa
    /// por el EventManager (GamePausedEvent / GameResumedEvent) y quien quiera mostrar un
    /// menú (PauseMenuUI) o sonar (SoundPlayer) reacciona a eso, sin que este script los
    /// conozca.
    ///
    /// ESC nunca pisa una pausa que abrió otro sistema (panel de nivel, victoria, derrota):
    /// si Time.timeScale ya estaba en 0 por otra razón cuando se intenta pausar, no hace
    /// nada — evita despausar por accidente algo que este script no abrió.
    /// </summary>
    public class PauseController : MonoBehaviour
    {
        private bool _isPaused;

        public bool IsPaused => _isPaused;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            if (_isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        public void Pause()
        {
            if (_isPaused || Time.timeScale == 0f)
            {
                return;
            }

            _isPaused = true;
            Time.timeScale = 0f;
            EventManager.Publish(new GamePausedEvent());
        }

        public void Resume()
        {
            if (!_isPaused)
            {
                return;
            }

            _isPaused = false;
            Time.timeScale = 1f;
            EventManager.Publish(new GameResumedEvent());
        }

        public void Restart()
        {
            _isPaused = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnDisable()
        {
            if (_isPaused)
            {
                _isPaused = false;
                Time.timeScale = 1f;
            }
        }
    }
}
