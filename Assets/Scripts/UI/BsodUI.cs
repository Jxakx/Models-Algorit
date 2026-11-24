using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Localization;
using ArenaSurvivor.Managers;

namespace ArenaSurvivor.UI
{
    // El "pantallazo azul" cuando el jugador muere 
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
