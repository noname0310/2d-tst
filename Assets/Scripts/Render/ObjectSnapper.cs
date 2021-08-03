using UnityEngine;

namespace Assets.Scripts.Render
{
    [DisallowMultipleComponent]
    public class ObjectSnapper : MonoBehaviour
    {
        public float SnapUnit = 0.25f;
        public bool AutoZaxisSort = true;

        // ReSharper disable once UnusedMember.Local
        private void Awake() => Destroy(this);

        // ReSharper disable once UnusedMember.Local
        private void OnDrawGizmos()
        {
            transform.position = Snapping.Snap(transform.position, new Vector3(SnapUnit, SnapUnit, SnapUnit));
            if (!AutoZaxisSort) return;
            var zaxisSorter = GetComponent<ZaxisSorter>();
            if (zaxisSorter == null)
                zaxisSorter = gameObject.AddComponent<ZaxisSorter>();
            zaxisSorter.SortYaxis();
        }
    }
}
