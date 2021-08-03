using UnityEngine;
using Assets.Scripts.Event;

namespace Assets.Scripts.Loader
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public class CanvasCameraInitializer : MonoBehaviour
    {
        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            GetComponent<Canvas>().worldCamera = EventManager.Camera;
            Destroy(this);
        }
    }
}
