#nullable enable
using System;
using Assets.Scripts.Loader;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UiEventManager
    {
        public event Action<Vector2>? OnMove;
        public event Action? OnInteractionButtonClicked;
        public event Action? OnPause;
        public event Action? OnDialogButtonClicked;
        public bool BlockingLeftStickMove { get; set; }
        public bool BlockingInteraction { get; set; }
        public bool BlockingPause { get; set; }
        private Vector2 _lastMoveVector2;

        public UiEventManager()
        {
            BlockingLeftStickMove = false;
            BlockingInteraction = false;
            BlockingPause = false;
            SceneLoader.OnStageChanged += SceneLoader_OnStageChanged;
        }

        ~UiEventManager() => SceneLoader.OnStageChanged -= SceneLoader_OnStageChanged;

        private void SceneLoader_OnStageChanged() => OnMove?.Invoke(_lastMoveVector2);

        public void ControllerLeftStickMove(Vector2 vector2)
        {
            _lastMoveVector2 = vector2;
            if (!BlockingLeftStickMove)
                OnMove?.Invoke(vector2);
        }

        public void InteractionButtonClicked()
        {
            if (!BlockingInteraction)
                OnInteractionButtonClicked?.Invoke();
        }

        public void PauseButtonClicked()
        {
            if (!BlockingPause)
                OnPause?.Invoke();
        }

        public void DialogButtonClicked() => OnDialogButtonClicked?.Invoke();
    }
}
