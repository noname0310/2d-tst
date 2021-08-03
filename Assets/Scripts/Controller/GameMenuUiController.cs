using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

namespace Assets.Scripts.Controller
{
    public class GameMenuUiController : MonoBehaviour
    {
        public event Action<bool>? OnMenuActiveUpdated;
        public bool IsMenuOpened { get; private set; }
        public Animator? Animator;
        private bool _isSettingsOpened;
        private bool _isPopMessageShowing;
        private float _popupMessageTime;
        private static readonly int ShowMenuHash = Animator.StringToHash("ShowMenu");
        private static readonly int ShowSettingsHash = Animator.StringToHash("ShowSettings");
        private static readonly int ShowPopupMessageHash = Animator.StringToHash("ShowPopupMessage");
        private const float Duration = 2f;

        // ReSharper disable once UnusedMember.Local
        private void Awake() => GameManager.OnInputControlsChanged += ForceMenuActiveUpdate;

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy() => GameManager.OnInputControlsChanged -= ForceMenuActiveUpdate;

        private void ForceMenuActiveUpdate(PlayerInput playerInput) => OnMenuActiveUpdated?.Invoke(IsMenuOpened);

        public void GoBack()
        {
            if (_isSettingsOpened)
            {
                Animator!.SetBool(ShowSettingsHash, false);
                _isSettingsOpened = false;
                GameManager.Storage!.SaveSettings();
            }
            else
            {
                Animator!.SetBool(ShowMenuHash, false);
                IsMenuOpened = false;
                OnMenuActiveUpdated?.Invoke(IsMenuOpened);
            }
        }

        public void OpenMenu()
        {
            if (IsMenuOpened) return;
            Animator!.SetBool(ShowMenuHash, true);
            IsMenuOpened = true;
            OnMenuActiveUpdated?.Invoke(IsMenuOpened);
        }
        
        public void OpenSettings()
        {
            if (!IsMenuOpened) return;
            Animator!.SetBool(ShowSettingsHash, true);
            _isSettingsOpened = true;
        }

        public void ShowPopupMessage()
        {
            if (!_isPopMessageShowing)
            {
                _isPopMessageShowing = true;
                Animator!.SetBool(ShowPopupMessageHash, true);
                StartCoroutine(PopupMessageProcess(Duration));
            }
            else
                _popupMessageTime = 0;
        }

        private IEnumerator PopupMessageProcess(float duration)
        {
            _popupMessageTime = 0;
            while (_popupMessageTime <= duration)
            {
                yield return null;
                _popupMessageTime += Time.unscaledDeltaTime;
            }
            Animator!.SetBool(ShowPopupMessageHash, false);
            _isPopMessageShowing = false;
        }
    }
}
