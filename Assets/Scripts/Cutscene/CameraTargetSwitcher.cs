#nullable enable
using Assets.Scripts.Event;
using Assets.Scripts.Loader;
using Cinemachine;
using UnityEngine;

namespace Assets.Scripts.CutScene
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraTargetSwitcher : MonoBehaviour
    {
        public Transform? ManualTargetTransform;
        private CinemachineVirtualCamera? _cinemachineVirtualCamera;
        private Transform? _transform;

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
            _transform = _cinemachineVirtualCamera.Follow;
            SceneLoader.OnStageLoaded += SceneLoader_OnStageLoaded;
        }

        private void SceneLoader_OnStageLoaded()
        {
            EventManager.Cutscene!.OnCutsceneDirectorStart += Cutscene_OnCutsceneStart;
            EventManager.Cutscene!.OnCutsceneComplete += CameraTargetSwitcher_OnCutsceneComplete;
            SceneLoader.OnStageLoaded -= SceneLoader_OnStageLoaded;
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            if (EventManager.Cutscene == null) return;
            EventManager.Cutscene.OnCutsceneDirectorStart -= Cutscene_OnCutsceneStart;
            EventManager.Cutscene.OnCutsceneComplete -= CameraTargetSwitcher_OnCutsceneComplete;
        }

        private void Cutscene_OnCutsceneStart() => _cinemachineVirtualCamera!.Follow = ManualTargetTransform!;

        private void CameraTargetSwitcher_OnCutsceneComplete() => _cinemachineVirtualCamera!.Follow = _transform!;
    }
}