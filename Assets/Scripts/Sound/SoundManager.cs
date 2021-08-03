using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Sound
{
    public class SoundManager
    {
        public float SfxVolume
        {
            get
            {
                if (_audioMixer.GetFloat("SFX", out var value))
                    return value;
                throw new KeyNotFoundException();
            }
            set => _audioMixer.SetFloat("SFX", value);
        }

        public float MusicVolume
        {
            get
            {
                if (_audioMixer.GetFloat("Music", out var value))
                    return value;
                throw new KeyNotFoundException();
            }
            set => _audioMixer.SetFloat("Music", value);
        }

        private readonly AudioMixer _audioMixer;
        private readonly GlobalSound _globalSound;

        public SoundManager(AudioMixer audioMixer, GlobalSound globalSound) =>
            (_audioMixer, _globalSound) = (audioMixer, globalSound);

        public void ChangeGlobalSound(AudioClip audioClip) => _globalSound.ChangeSound(audioClip);

        public void StopGlobalSound() => _globalSound.Stop();
    }
}
