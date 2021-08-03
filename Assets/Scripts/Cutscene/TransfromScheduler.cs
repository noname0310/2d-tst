#nullable enable
using System;
using UnityEngine;

namespace Assets.Scripts.CutScene
{
    [DisallowMultipleComponent]
    public class TransfromScheduler: MonoBehaviour
    {
        private TransformTarget[]? _transformTargets;
        private int _pathCompleteCount;
        private Action? _onComplete;

        public void StartPositioning(GameObject eventObject, Action? onComplete)
        {
            _pathCompleteCount = 0;
            _onComplete = onComplete;
            _transformTargets = eventObject.GetComponentsInChildren<TransformTarget>();
            if (_transformTargets.Length == 0)
            {
                onComplete?.Invoke();
                return;
            }

            foreach (var item in _transformTargets)
            {
                item.OnPathComplete += Item_OnPathComplete;
                item.enabled = true;
            }
        }

        private void Item_OnPathComplete()
        {
            _pathCompleteCount += 1;
            if (_transformTargets!.Length != _pathCompleteCount) return;
            var completed = true;
            foreach (var item in _transformTargets)
            {
                if (item.CheckIsCompleted()) continue;
                _pathCompleteCount -= 1;
                completed = false;
                item.enabled = true;
            }

            if (!completed) return;
            foreach (var item in _transformTargets)
                item.OnPathComplete -= Item_OnPathComplete;
            _onComplete?.Invoke();
        }
    }
}
