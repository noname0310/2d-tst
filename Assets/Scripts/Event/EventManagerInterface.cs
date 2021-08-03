using Assets.Scripts.Loader;
using UnityEngine;

namespace Assets.Scripts.Event
{
    public class EventManagerInterface : MonoBehaviour
    {
        public void GameOver() => EventManager.Instance?.GameOver();

        public void ChangeGlobalSound(AudioClip audioClip) => GameManager.Sound?.ChangeGlobalSound(audioClip);

        public void LoadCredit() => SceneLoader.LoadCredit();
    }
}
