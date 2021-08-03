using UnityEngine;

namespace Assets.Scripts.Loader
{
    public class TitleMenuBootstraper : MonoBehaviour
    {
        // ReSharper disable once UnusedMember.Local
        private void Awake() =>
            GameManager.SetInputMap("Player");
    }
}
