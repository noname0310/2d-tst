using UnityEngine;
using Assets.Scripts.Event;

namespace Assets.Scripts.Loader
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class CameraRegister : MonoBehaviour
    {
        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            EventManager.Camera = GetComponent<Camera>();
            Destroy(this);
        }
    }
}
