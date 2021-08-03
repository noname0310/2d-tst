#nullable enable
using System;
using UnityEngine;
using Assets.Scripts.Controller;
using Assets.Scripts.Render;

namespace Assets.Scripts.CutScene
{
    [DisallowMultipleComponent]
    public class TransformTarget : MonoBehaviour
    {
        public event Action? OnPathComplete;
        public AiPathController? Target;
        private const float AiPathingDistance = 0.8f;
        private const float MinDistance = 0.0001f;
        private const float DrawZoneSize = 0.3f;

        public bool CheckIsCompleted() => Vector2.Distance(transform.position, Target!.transform.position) < MinDistance;

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            var distance = Vector2.Distance(transform.position, Target!.transform.position);
            if (!(distance < AiPathingDistance))
            {
                Target.SetPath(transform);
                return;
            }
            Target.StopPath();
            Target.ForceMoveStep(transform.position, Time.deltaTime);
            if (!(distance < MinDistance)) return;
            Target.ForceMoveEnd();
            OnPathComplete?.Invoke();
            enabled = false;
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDrawGizmosSelected() => GizmoDrawer.DrawWireDisk(transform, DrawZoneSize);
    }
}
