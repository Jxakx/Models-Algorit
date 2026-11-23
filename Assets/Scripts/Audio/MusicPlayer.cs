using UnityEngine;
using ArenaSurvivor.Core.Audio;

namespace ArenaSurvivor.Audio
{
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
