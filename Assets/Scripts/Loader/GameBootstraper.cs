using UnityEngine;

namespace Assets.Scripts.Loader
{
    [DisallowMultipleComponent]
    public class GameBootstraper : MonoBehaviour
    {
        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            SceneLoader.LoadTitle();
            Destroy(this);
        }
    }
}
