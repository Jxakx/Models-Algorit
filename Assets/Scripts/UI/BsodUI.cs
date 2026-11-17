using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Localization;
using ArenaSurvivor.Managers;

namespace ArenaSurvivor.UI
{
    /// <summary>
    /// El "pantallazo azul" cuando el jugador muere — reemplaza al panel de game over
    /// genérico solo para la derrota (ganar sigue mostrando el panel normal de GameEndUI).
    /// Vive separado de GameEndUI a propósito: son dos tratamientos visuales completamente
    /// distintos, y cada uno solo necesita escuchar su propio evento. El texto sale del
    /// sistema de localización. Con el panel activo, R reinicia la escena.
    /// </summary>
    public class BsodUI : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TextMeshProUGUI messageText;

        private bool _isShowingPanel;

        private void Awake()
        {
            panelRoot.SetActive(false);
        }

        private void OnEnable()
        {
            EventManager.Subscribe<GameOverEvent>(OnGameOver);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<GameOverEvent>(OnGameOver);
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
            // La carita triste grande de arriba, como en el pantallazo azul de Windows.
            messageText.text = "<size=140>:(</size>\n\n" + Localizer.Get(LocKeys.BsodMessage);

            _isShowingPanel = true;
            panelRoot.SetActive(true);
        }
    }
}
