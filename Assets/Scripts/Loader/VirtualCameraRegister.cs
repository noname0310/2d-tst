using UnityEngine;
using Cinemachine;
using Assets.Scripts.Event;

namespace Assets.Scripts.Loader
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class VirtualCameraRegister : MonoBehaviour
    {
        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            EventManager.CinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
            Destroy(this);
        }
    }
}