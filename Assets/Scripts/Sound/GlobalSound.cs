#nullable enable
using UnityEngine;
using Assets.Scripts.UI;

namespace Assets.Scripts.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class GlobalSound : MonoBehaviour
    {
        private AudioSource? _audioSource;
        private Coroutine? _coroutine;
        private const float AudioFadeInDuration = 1f;
        private const float AudioFadeOutDuration = 1f;

        // ReSharper disable once UnusedMember.Local
        private void Awake() => _audioSource = GetComponent<AudioSource>();

        public void ChangeSound(AudioClip audioClip)
        {
            if (_audioSource!.isPlaying && _audioSource.clip == audioClip)
                return;
            if (_audioSource.isPlaying)
            {
                if (_coroutine != null)
                    StopCoroutine(_coroutine);
                _coroutine = StartCoroutine(CoroutineAnimation.FadeAudio(_audioSource, false, AudioFadeOutDuration, () =>
                {
                    _audioSource.Stop();
                    _audioSource.clip = audioClip;
                    _audioSource.Play();
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine = StartCoroutine(CoroutineAnimation.FadeAudio(_audioSource, true, AudioFadeInDuration, null));
                }));
            }
            else
            {
                _audioSource.volume = 1f;
                _audioSource.clip = audioClip;
                _audioSource.Play();
            }
        }

        public void Stop()
        {
            if (!_audioSource!.isPlaying) return;
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(CoroutineAnimation.FadeAudio(_audioSource, false, AudioFadeOutDuration,
                () => { _audioSource.Stop(); }));
        }
    }
}
