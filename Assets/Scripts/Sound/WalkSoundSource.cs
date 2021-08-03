#nullable enable
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Sound
{
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class WalkSoundSource : MonoBehaviour
    {
        public List<AudioClip>? StepSounds;
        public float StepSoundDistance = 2f;
        private AudioSource? _audioSource;
        private Vector2 _lastPosition;
        private float _movedDiatance;

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _lastPosition = transform.position;
            _movedDiatance = 0f;
        }

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            _movedDiatance += Vector2.Distance(_lastPosition, transform.position);
            _lastPosition = transform.position;

            if (!(StepSoundDistance < _movedDiatance)) return;
            _audioSource!.PlayOneShot(StepSounds![Random.Range(0, StepSounds.Count)]);
            _movedDiatance = 0f;
        }
    }
}
