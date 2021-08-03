#nullable enable
using UnityEngine;
using Assets.Scripts.Loader;
using Assets.Scripts.Render;

namespace Assets.Scripts.Event
{
    [DefaultExecutionOrder(2)]
    [DisallowMultipleComponent]
    public class StageEntryPoint : MonoBehaviour
    {
        public string? EntryName;
        private const float DrawZoneSize = 0.2f;

        // ReSharper disable once UnusedMember.Local
        private void Awake() => SceneLoader.OnStageInitalize += SceneLoader_OnStageInitalize;

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy() => SceneLoader.OnStageInitalize -= SceneLoader_OnStageInitalize;

        private void SceneLoader_OnStageInitalize()
        {
            if (ComponentReferenceHolder.EntryPointName != null && EntryName != null &&
                ComponentReferenceHolder.EntryPointName == EntryName)
                EventManager.Player!.transform.position = transform.position;
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDrawGizmosSelected() => GizmoDrawer.DrawWireDisk(transform, DrawZoneSize);
    }
}
