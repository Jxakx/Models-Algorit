using System;
using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Core.Audio;

namespace ArenaSurvivor.Audio
{
    /// <summary>Una fila de la Look-Up Table de sonidos: un nombre (SoundId) y el clip que suena con él.</summary>
    [Serializable]
    public class SoundEntry
    {
        public SoundId id;

        [Tooltip("Arrastrá acá el audio de este sonido. Vacío = ese sonido queda en silencio.")]
        public AudioClip clip;

        [Range(0f, 1f)] public float volume = 1f;
    }

    /// <summary>
    /// Look-Up Table de sonidos: asocia cada nombre (SoundId) con su clip y su volumen.
    /// Quien quiere que suene algo pide un SoundId y la tabla responde en O(1) con el clip —
    /// ningún script del juego carga un audio por nombre ni por ruta. Los clips se arrastran
    /// a mano en el Inspector, y el volumen general y el de cada sonido se calibran acá
    /// mismo, en un único lugar.
    ///
    /// La lista siempre muestra una fila por cada SoundId (si se agrega uno nuevo al enum,
    /// aparece solo la próxima vez que se toque el Inspector).
    /// </summary>
    public class SoundBank : MonoBehaviour
    {
        [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;
        [SerializeField] private List<SoundEntry> sounds = new List<SoundEntry>();

        private Dictionary<SoundId, SoundEntry> _lookup;

        public float MasterVolume => masterVolume;

        private void Awake()
        {
            _lookup = BuildLookup();
        }

        private void OnValidate()
        {
            EnsureAllEntries();
            _lookup = null;
        }

        /// <summary>Busca el sonido por nombre. Devuelve false si no tiene clip asignado.</summary>
        public bool TryGet(SoundId id, out SoundEntry entry)
        {
            _lookup ??= BuildLookup();
            return _lookup.TryGetValue(id, out entry) && entry.clip != null;
        }

        /// <summary>Deja una fila por cada SoundId, en el orden del enum, sin perder lo ya asignado.</summary>
        [ContextMenu("Completar entradas que falten")]
        public void EnsureAllEntries()
        {
            var ordered = new List<SoundEntry>();

            foreach (SoundId id in Enum.GetValues(typeof(SoundId)))
            {
                var existing = sounds.Find(entry => entry != null && entry.id == id);
                ordered.Add(existing ?? new SoundEntry { id = id });
            }

            var changed = ordered.Count != sounds.Count;
            for (var i = 0; i < ordered.Count && !changed; i++)
            {
                changed = !ReferenceEquals(ordered[i], sounds[i]);
            }

            if (changed)
            {
                sounds = ordered;
            }
        }

        private Dictionary<SoundId, SoundEntry> BuildLookup()
        {
            var lookup = new Dictionary<SoundId, SoundEntry>();
            foreach (var entry in sounds)
            {
                if (entry != null)
                {
                    lookup[entry.id] = entry;
                }
            }

            return lookup;
        }
    }
}
