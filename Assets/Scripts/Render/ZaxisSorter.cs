using UnityEngine;

namespace Assets.Scripts.Render
{
    [DisallowMultipleComponent]
    public class ZaxisSorter : MonoBehaviour
    {
        public float Offset = 0f;
        public bool RunOnce = true;
        public bool RunOnEditor = true;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            if (!RunOnce) return;
            LateUpdate();
            Destroy(this);
        }

        // ReSharper disable once UnusedMember.Local
        private void LateUpdate() => transform.position =
            new Vector3(transform.position.x, transform.position.y, transform.position.y + Offset);


        // ReSharper disable once UnusedMember.Local
        private void OnDrawGizmos()
        {
            if (RunOnEditor)
                SortYaxis();
        }

        public void SortYaxis() => LateUpdate();
    }
}
