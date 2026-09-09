using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Core.Combat;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Player;
using ArenaSurvivor.UI;
using ArenaSurvivor.Weapons;

namespace ArenaSurvivor.Leveling
{
    /// <summary>
    /// Al subir de nivel, pausa el juego y muestra 3 opciones al azar entre: desbloquear un
    /// arma nueva, mejorar una que ya tenga, o subir un stat. Al elegir una, la aplica,
    /// cierra el panel y reanuda. Las mejoras en sí (Apply) viven en UpgradeChoice.
    /// </summary>
    public class LevelUpManager : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private LevelUpButtonUI[] buttons;
        [SerializeField] private WeaponBase[] weapons;
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private Health playerHealth;

        private readonly List<UpgradeChoice> _currentChoices = new List<UpgradeChoice>();

        private void Awake()
        {
            panelRoot.SetActive(false);

            for (var i = 0; i < buttons.Length; i++)
            {
                var index = i;
                buttons[i].Button.onClick.AddListener(() => OnChoicePicked(index));
            }
        }

        private void OnEnable()
        {
            EventManager.Subscribe<PlayerLeveledUpEvent>(OnLeveledUp);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<PlayerLeveledUpEvent>(OnLeveledUp);
        }

        private void OnLeveledUp(PlayerLeveledUpEvent gameEvent)
        {
            var pool = BuildChoicePool();
            Shuffle(pool);

            _currentChoices.Clear();
            var count = Mathf.Min(buttons.Length, pool.Count);
            for (var i = 0; i < count; i++)
            {
                _currentChoices.Add(pool[i]);
            }

            for (var i = 0; i < buttons.Length; i++)
            {
                var hasChoice = i < _currentChoices.Count;
                buttons[i].gameObject.SetActive(hasChoice);
                if (hasChoice)
                {
                    buttons[i].SetTitle(_currentChoices[i].Title);
                }
            }

            panelRoot.SetActive(true);
            Time.timeScale = 0f;
        }

        private void OnChoicePicked(int index)
        {
            if (index >= _currentChoices.Count)
            {
                return;
            }

            _currentChoices[index].Apply();
            panelRoot.SetActive(false);
            Time.timeScale = 1f;
        }

        private List<UpgradeChoice> BuildChoicePool()
        {
            var pool = new List<UpgradeChoice>();

            foreach (var weapon in weapons)
            {
                if (!weapon.enabled)
                {
                    var weaponToUnlock = weapon;
                    pool.Add(new UpgradeChoice(
                        $"Nueva arma: {weaponToUnlock.Data.WeaponName}",
                        () => weaponToUnlock.enabled = true));
                }
                else if (!weapon.IsMaxLevel)
                {
                    var weaponToLevelUp = weapon;
                    pool.Add(new UpgradeChoice(
                        $"{weaponToLevelUp.Data.WeaponName} Nv.{weaponToLevelUp.CurrentLevel + 1}",
                        () => weaponToLevelUp.LevelUp()));
                }
            }

            pool.Add(new UpgradeChoice("Velocidad +10%", () => playerStats.AddMoveSpeedPercent(0.1f)));
            pool.Add(new UpgradeChoice("Daño +10%", () => playerStats.AddDamagePercent(0.1f)));
            pool.Add(new UpgradeChoice("Vida máxima +20", () => playerHealth.ConfigureMaxHealth(playerHealth.MaxHealth + 20f)));

            return pool;
        }

        private static void Shuffle(IList<UpgradeChoice> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
