#nullable enable
using UnityEngine;
using Assets.Scripts.UI;

namespace Assets.Scripts.Sound
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class AudioFadeOut : MonoBehaviour
    {
        public float AudioFadeOutDuration = 0.5f;
        private AudioSource? _audioSource;

        // ReSharper disable once UnusedMember.Local
        private void Awake() => _audioSource = GetComponent<AudioSource>();

        public void FadeOut() =>
            StartCoroutine(CoroutineAnimation.FadeAudio(_audioSource!, false, AudioFadeOutDuration,
                () => { _audioSource!.Stop(); }));
    }
}
