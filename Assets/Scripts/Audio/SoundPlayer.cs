using UnityEngine;
using ArenaSurvivor.Core.Audio;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Enemies;
using ArenaSurvivor.Fx;
using ArenaSurvivor.Leveling;
using ArenaSurvivor.Managers;
using ArenaSurvivor.Player;
using ArenaSurvivor.UI;

namespace ArenaSurvivor.Audio
{
    /// <summary>
    /// Traduce eventos de juego en efectos de sonido puntuales. Cada evento se mapea a un
    /// SoundId y el audio real sale de la SoundBank (Look-Up Table): este script nunca
    /// referencia un AudioClip. Un solo AudioSource con PlayOneShot alcanza: varios efectos
    /// cortos superpuestos no se cortan entre sí.
    ///
    /// Es oyente del EventManager (Observer): los sistemas que generan los eventos no
    /// saben que hay sonido.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private SoundBank bank;

        [Tooltip("Intervalo mínimo entre dos impactos/muertes seguidos, para que una " +
                 "muerte múltiple no sature el oído con el mismo sonido pegado.")]
        [SerializeField] private float minIntervalForFrequentSfx = 0.05f;

        [Tooltip("Variación de tono al azar en cada sonido (±), para que no suene igual de repetitivo.")]
        [SerializeField] private float pitchVariation = 0.05f;

        private AudioSource _source;
        private float _lastKnownPlayerHealth = -1f;
        private float _lastImpactTime = float.NegativeInfinity;
        private float _lastDeathTime = float.NegativeInfinity;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _source.playOnAwake = false;
        }

        private void OnEnable()
        {
            EventManager.Subscribe<WeaponFiredEvent>(OnWeaponFired);
            EventManager.Subscribe<WeaponImpactEvent>(OnWeaponImpact);
            EventManager.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
            EventManager.Subscribe<PlayerHealthChangedEvent>(OnPlayerHealthChanged);
            EventManager.Subscribe<XpCollectedEvent>(OnXpCollected);
            EventManager.Subscribe<ChoicePanelOpenedEvent>(OnPanelOpened);
            EventManager.Subscribe<UpgradeChosenEvent>(OnUpgradeChosen);
            EventManager.Subscribe<GameOverEvent>(OnGameOver);
            EventManager.Subscribe<GameWonEvent>(OnGameWon);
            EventManager.Subscribe<GamePausedEvent>(OnPaused);
            EventManager.Subscribe<GameResumedEvent>(OnResumed);
            EventManager.Subscribe<UiButtonClickedEvent>(OnButtonClicked);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<WeaponFiredEvent>(OnWeaponFired);
            EventManager.Unsubscribe<WeaponImpactEvent>(OnWeaponImpact);
            EventManager.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
            EventManager.Unsubscribe<PlayerHealthChangedEvent>(OnPlayerHealthChanged);
            EventManager.Unsubscribe<XpCollectedEvent>(OnXpCollected);
            EventManager.Unsubscribe<ChoicePanelOpenedEvent>(OnPanelOpened);
            EventManager.Unsubscribe<UpgradeChosenEvent>(OnUpgradeChosen);
            EventManager.Unsubscribe<GameOverEvent>(OnGameOver);
            EventManager.Unsubscribe<GameWonEvent>(OnGameWon);
            EventManager.Unsubscribe<GamePausedEvent>(OnPaused);
            EventManager.Unsubscribe<GameResumedEvent>(OnResumed);
            EventManager.Unsubscribe<UiButtonClickedEvent>(OnButtonClicked);
        }

        private void OnWeaponFired(WeaponFiredEvent gameEvent) => Play(gameEvent.Sound);

        private void OnWeaponImpact(WeaponImpactEvent gameEvent)
        {
            if (Time.unscaledTime - _lastImpactTime < minIntervalForFrequentSfx)
            {
                return;
            }

            _lastImpactTime = Time.unscaledTime;
            Play(SoundId.Impact);
        }

        private void OnEnemyKilled(EnemyKilledEvent gameEvent)
        {
            if (Time.unscaledTime - _lastDeathTime < minIntervalForFrequentSfx)
            {
                return;
            }

            _lastDeathTime = Time.unscaledTime;
            Play(SoundId.EnemyDeath);
        }

        private void OnPlayerHealthChanged(PlayerHealthChangedEvent gameEvent)
        {
            // El primer evento (estado inicial al arrancar) no debe sonar; el centinela
            // negativo hace que ese primer chequeo se salte solo.
            if (_lastKnownPlayerHealth >= 0f && gameEvent.CurrentHealth < _lastKnownPlayerHealth)
            {
                Play(SoundId.PlayerHurt);
            }

            _lastKnownPlayerHealth = gameEvent.CurrentHealth;
        }

        private void OnXpCollected(XpCollectedEvent gameEvent) => Play(SoundId.XpPickup);

        private void OnPanelOpened(ChoicePanelOpenedEvent gameEvent) => Play(SoundId.LevelUp);

        private void OnUpgradeChosen(UpgradeChosenEvent gameEvent) => Play(SoundId.UpgradeConfirm);

        private void OnGameOver(GameOverEvent gameEvent) => Play(SoundId.GameOver);

        private void OnGameWon(GameWonEvent gameEvent) => Play(SoundId.Victory);

        private void OnPaused(GamePausedEvent gameEvent) => Play(SoundId.PauseOpen);

        private void OnResumed(GameResumedEvent gameEvent) => Play(SoundId.PauseClose);

        private void OnButtonClicked(UiButtonClickedEvent gameEvent) => Play(SoundId.ButtonClick);

        /// <summary>Reproduce el sonido de ese nombre si la tabla tiene un clip asignado; si no, no hace nada.</summary>
        public void Play(SoundId id)
        {
            if (bank == null || !bank.TryGet(id, out var entry))
            {
                return;
            }

            _source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            _source.PlayOneShot(entry.clip, entry.volume * bank.MasterVolume);
        }
    }
}
