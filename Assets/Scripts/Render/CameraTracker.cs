#nullable enable
using System;
using UnityEngine;
using Assets.Scripts.Event;
using Assets.Scripts.Loader;

namespace Assets.Scripts.Render
{
    [DisallowMultipleComponent]
    public class CameraTracker : MonoBehaviour
    {
        public GameObject? TrackableGo;
        public Vector3 Offset = new Vector3(0, 0, -10);
        public float SmoothSpeed = 0.1f;
        public float VectorZeroSmoothSpeed = 0.01f;
        public float LookDistance = 1f;
        public float MovingLookDistance = 0.5f;
        public bool AutoInitalize = true;

        private GameObject? _trackableGoChange;
        private ITrackable? _trackable;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            if (AutoInitalize)
            {
                transform.position = EventManager.Player!.transform.position + Offset;
                TrackableGo = EventManager.Player;
                SceneLoader.OnStageChange += SceneLoader_OnStageChange;
                SceneLoader.OnStageChanged += SceneLoader_OnStageChanged;
            }
            _trackableGoChange = TrackableGo;
            if (!TrackableGo!.TryGetComponent(out _trackable))
                throw new Exception("GetComponentFailed");
        }

        private void SceneLoader_OnStageChanged()
        {
            transform.position = EventManager.Player!.transform.position + Offset;
            TrackableGo = EventManager.Player;
            enabled = true;
        }

        private void SceneLoader_OnStageChange() => enabled = false;

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            if (!AutoInitalize)
                return;

            SceneLoader.OnStageChange -= SceneLoader_OnStageChange;
            SceneLoader.OnStageChanged -= SceneLoader_OnStageChanged;
        }

        // ReSharper disable once UnusedMember.Local
        private void FixedUpdate()
        {
            if (TrackableGo != _trackableGoChange)
            {
                _trackableGoChange = TrackableGo;
                if (!TrackableGo!.TryGetComponent(out _trackable))
                    throw new Exception("GetComponentFailed");
            }

            var target = _trackable!.Position;
            target += Offset + (Vector3)(_trackable.LookVector * LookDistance + _trackable.MoveVector * MovingLookDistance);
            transform.position = Vector3.Lerp(transform.position, target,
                _trackable.MoveVector == Vector2.zero
                    ? VectorZeroSmoothSpeed
                    : SmoothSpeed);
        }
    }
}
