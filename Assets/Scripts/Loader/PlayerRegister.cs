using UnityEngine;
using Assets.Scripts.Event;

namespace Assets.Scripts.Loader
{
    [DisallowMultipleComponent]
    public class PlayerRegister : MonoBehaviour
    {
        // ReSharper disable once UnusedMember.Local
        private void Awake() => EventManager.Player = gameObject;
        // ReSharper disable once UnusedMember.Local
        private void OnDestroy() => EventManager.Player = null;
    }
}
