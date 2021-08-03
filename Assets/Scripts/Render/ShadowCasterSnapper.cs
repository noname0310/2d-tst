using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Assets.Scripts.Render
{
    [RequireComponent(typeof(ShadowCaster2D))]
    [DisallowMultipleComponent]
    public class ShadowCasterSnapper : MonoBehaviour
    {
        public float SnapUnit = 1f;

        // ReSharper disable once UnusedMember.Local
        private void Awake() => Destroy(this);

        // ReSharper disable once UnusedMember.Local
        private void OnDrawGizmos()
        {
            var field = typeof(ShadowCaster2D).GetField(
                "m_ShapePath", 
                BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            if (field is null) return;
            var mashPaths = (Vector3[]) field.GetValue(GetComponent<ShadowCaster2D>());
            for (var index = 0; index < mashPaths.Length; index++)
                mashPaths[index] = Snapping.Snap(mashPaths[index], new Vector2(SnapUnit, SnapUnit));
        }
    }
}
