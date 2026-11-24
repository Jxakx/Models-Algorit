using UnityEngine;
using UnityEngine.SceneManagement;
using ArenaSurvivor.Core.Events;

namespace ArenaSurvivor.Managers
{
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
