using UnityEngine;

namespace Assets.Scripts.Event
{
    public class Teleporter : MonoBehaviour
    {
        public void Teleport(Transform target) => transform.position = target.position;
    }
}
