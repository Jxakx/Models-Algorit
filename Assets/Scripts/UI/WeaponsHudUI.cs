using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Localization;
using ArenaSurvivor.Weapons;

namespace ArenaSurvivor.UI
{
    /// <summary>
    /// Lista en el HUD las armas que el jugador ya tiene y a qué nivel están. No conoce a
    /// WeaponBase ni al Player — solo escucha WeaponChangedEvent (Observer) y arma su propia
    /// tabla clave de nombre → nivel, así que una arma nueva aparece sola apenas se
    /// desbloquea. Guarda la CLAVE del nombre y la traduce al dibujar, así la lista se
    /// vuelve a escribir sola cuando cambia el idioma.
    /// </summary>
    public class WeaponsHudUI : MonoBehaviour
    {
        [SerializeField] private Text weaponsText;

        // Ordenado por orden de aparición (la primera arma que avisa queda arriba de la lista).
        private readonly List<string> _order = new List<string>();
        private readonly Dictionary<string, int> _levelByWeapon = new Dictionary<string, int>();
        private readonly StringBuilder _builder = new StringBuilder();

        private void OnEnable()
        {
            EventManager.Subscribe<WeaponChangedEvent>(OnWeaponChanged);
            EventManager.Subscribe<LanguageChangedEvent>(OnLanguageChanged);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe<WeaponChangedEvent>(OnWeaponChanged);
            EventManager.Unsubscribe<LanguageChangedEvent>(OnLanguageChanged);
        }

        private void OnWeaponChanged(WeaponChangedEvent gameEvent)
        {
            if (!_levelByWeapon.ContainsKey(gameEvent.WeaponName))
            {
                _order.Add(gameEvent.WeaponName);
            }

            _levelByWeapon[gameEvent.WeaponName] = gameEvent.Level;
            Redraw();
        }

        private void OnLanguageChanged(LanguageChangedEvent gameEvent) => Redraw();

        private void Redraw()
        {
            _builder.Clear();

            for (var i = 0; i < _order.Count; i++)
            {
                var weaponKey = _order[i];
                if (i > 0)
                {
                    _builder.Append('\n');
                }

                _builder.Append(Localizer.Get(LocKeys.HudWeaponLevel, Localizer.Get(weaponKey), _levelByWeapon[weaponKey]));
            }

            weaponsText.text = _builder.ToString();
        }
    }
}
