using UnityEngine;
using ArenaSurvivor.Core.Audio;

namespace ArenaSurvivor.Audio
{
    /// <summary>
    /// Música de fondo en loop. Pide su clip a la SoundBank con el nombre SoundId.Music (no
    /// referencia ningún AudioClip), así que la canción se elige en el Inspector de la
    /// tabla. Si todavía no hay clip asignado, queda en silencio y se activa sola apenas se
    /// asigne uno (también en pleno Play).
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private SoundBank bank;

        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _source.loop = true;
            _source.playOnAwake = false;
        }

        private void Update()
        {
            if (bank == null || !bank.TryGet(SoundId.Music, out var entry))
            {
                return;
            }

            // El volumen sigue en vivo a los sliders de la SoundBank: calibrarlo en el
            // Inspector durante Play se escucha al toque.
            _source.volume = entry.volume * bank.MasterVolume;

            if (_source.clip != entry.clip)
            {
                _source.clip = entry.clip;
                _source.Play();
            }
            else if (!_source.isPlaying)
            {
                _source.Play();
            }
        }
    }
}
