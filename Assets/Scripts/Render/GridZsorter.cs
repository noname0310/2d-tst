using UnityEngine;
using Assets.Scripts.Event;

namespace Assets.Scripts.Render
{
    [DisallowMultipleComponent]
    public class GridZsorter : MonoBehaviour
    {
        private const float Distance = 100f;

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            if (EventManager.CinemachineVirtualCamera != null)
                transform.position = new Vector3(transform.position.x, transform.position.y,
                    EventManager.CinemachineVirtualCamera.transform.position.z + Distance);
        }
    }
}
