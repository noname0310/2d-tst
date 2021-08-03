using UnityEngine;

namespace Assets.Scripts.Render
{
    internal static class GizmoDrawer
    {
        public static void DrawWireDisk(Transform transform, float size, int corners = 50)
        {
            var origin = transform.position;
            var startRotation = transform.right * size;
            var lastPosition = origin + startRotation;
            var angle = 0.0f;
            while (angle <= 360)
            {
                angle += 360 / (float)corners;
                var nextPosition = origin + (Quaternion.Euler(0, 0, angle) * startRotation);
                Gizmos.DrawLine(lastPosition, nextPosition);
                lastPosition = nextPosition;
            }
        }
    }
}
