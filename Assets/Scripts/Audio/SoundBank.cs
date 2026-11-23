using System;
using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Core.Audio;

namespace ArenaSurvivor.Audio
{
    [Serializable]
    public class SoundEntry
    {
        public SoundId id;

        [Tooltip("Arrastrá acá el audio de este sonido. Vacío = ese sonido queda en silencio.")]
        public AudioClip clip;

        [Range(0f, 1f)] public float volume = 1f;
    }
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

        //Busca el sonido por nombre. Devuelve false si no tiene clip asignado
        public bool TryGet(SoundId id, out SoundEntry entry)
        {
            _lookup ??= BuildLookup();
            return _lookup.TryGetValue(id, out entry) && entry.clip != null;
        }

        //Deja una fila por cada SoundId, en el orden del enum, sin perder lo ya asignado
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
