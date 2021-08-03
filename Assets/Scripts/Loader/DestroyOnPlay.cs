using UnityEngine;

namespace Assets.Scripts.Loader
{
    [DisallowMultipleComponent]
    public class DestroyOnPlay : MonoBehaviour
    {
        public void Awake() => Destroy(gameObject);
    }
}
