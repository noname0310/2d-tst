#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class NavigationCursor : MonoBehaviour
    {
        public EventSystem? EventSystem;
        public List<Selectable>? DefaultControls;

        [SerializeField]
        private bool _isWorking;
        
        public bool AllowKeyboard { get; set; }
        private RectTransform? _rectTransform;
        private RectTransform? _selectedGameObjectRectTransform;
        private CanvasGroup? _canvasGroup;
        private Coroutine? _coroutine;
        private const float CursorSpeed = 0.4f;

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();

            if (!_isWorking)
            {
                DisableImmidiate();
                return;
            }

            GameManager.OnInputControlsChanged += GameManager_OnInputControlsChanged;
            GameManager_OnInputControlsChangedImmidiate(GameManager.CurrentControlScheme);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy() => GameManager.OnInputControlsChanged -= GameManager_OnInputControlsChanged;

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            if (_selectedGameObjectRectTransform == null)
                InitCursor();
            else
            {
                if (EventSystem!.currentSelectedGameObject == null || !EventSystem!.currentSelectedGameObject.activeInHierarchy)
                    InitCursor();
                if (EventSystem!.currentSelectedGameObject != null &&
                    _selectedGameObjectRectTransform != null &&
                    _selectedGameObjectRectTransform.gameObject != EventSystem!.currentSelectedGameObject)
                    _selectedGameObjectRectTransform =
                        EventSystem.currentSelectedGameObject.GetComponent<RectTransform>();
            }

            if (_selectedGameObjectRectTransform == null)
                return;

            _rectTransform!.anchorMax = Vector3.Lerp(_rectTransform.anchorMax, _selectedGameObjectRectTransform.anchorMax, CursorSpeed);
            _rectTransform.anchorMin = Vector3.Lerp(_rectTransform.anchorMin, _selectedGameObjectRectTransform.anchorMin, CursorSpeed);
            _rectTransform.sizeDelta = Vector3.Lerp(_rectTransform.sizeDelta, _selectedGameObjectRectTransform.sizeDelta, CursorSpeed);
            _rectTransform.pivot = Vector3.Lerp(_rectTransform.pivot, _selectedGameObjectRectTransform.pivot, CursorSpeed);
            transform.position = Vector3.Lerp(transform.position, _selectedGameObjectRectTransform.transform.position + new Vector3(0f, 0f, -0.5f), CursorSpeed);
        }

        private void GameManager_OnInputControlsChanged(PlayerInput obj)
        {
            switch (obj.currentControlScheme)
            {
                case "Touch":
                case "Keyboard&Mouse" when !AllowKeyboard:
                    Disable(null);
                    break;
                default:
                    Enable();
                    break;
            }
        }

        private void GameManager_OnInputControlsChanged(string currentControlScheme)
        {
            switch (currentControlScheme)
            {
                case "Touch":
                case "Keyboard&Mouse" when !AllowKeyboard:
                    Disable(null);
                    break;
                default:
                    Enable();
                    break;
            }
        }

        private void GameManager_OnInputControlsChangedImmidiate(string currentControlScheme)
        {
            switch (currentControlScheme)
            {
                case "Touch":
                case "Keyboard&Mouse" when !AllowKeyboard:
                    DisableImmidiate();
                    break;
                default:
                    Enable();
                    break;
            }
        }

        private void DisableImmidiate()
        {
            _canvasGroup!.alpha = 0;
            gameObject.SetActive(false);
        }

        private void InitCursor()
        {
            var isDefaultControlExist = false;
            for (var index = 0; index < DefaultControls!.Count; index++)
            {
                var defaultControl = DefaultControls![index];
                if (!defaultControl.IsInteractable() || !defaultControl.gameObject.activeInHierarchy) continue;
                isDefaultControlExist = true;
                defaultControl!.Select();
                _selectedGameObjectRectTransform = defaultControl.GetComponent<RectTransform>();
                break;
            }

            if (!isDefaultControlExist)
                _selectedGameObjectRectTransform = null;
        }

        private void Enable()
        {
            if (gameObject.activeSelf)
                return;
            gameObject.SetActive(true);
            if (!gameObject.activeInHierarchy)
                _canvasGroup!.alpha = 1f;
            else
            {
                if (_coroutine != null)
                    StopCoroutine(_coroutine);
                _coroutine =
                    StartCoroutine(CoroutineAnimation.FadeCanvasGroupAlpha(_canvasGroup!, true, 0.2f, null));
            }
        }

        public bool TryEnable()
        {
            switch (GameManager.CurrentControlScheme)
            {
                case "Touch":
                case "Keyboard&Mouse" when !AllowKeyboard:
                    return false;
                default:
                    Enable();
                    return true;
            }
        }

        public void Disable(Action? onComplete)
        {
            if (!gameObject.activeSelf)
                return;
            if (!gameObject.activeInHierarchy)
            {
                _canvasGroup!.alpha = 0f;
                gameObject.SetActive(false);
                onComplete?.Invoke();
            }
            else
            {
                if (_coroutine != null)
                    StopCoroutine(_coroutine);
                _coroutine =
                    StartCoroutine(CoroutineAnimation.FadeCanvasGroupAlpha(_canvasGroup!, false, 0.2f,
                        () =>
                        {
                            gameObject.SetActive(false);
                            onComplete?.Invoke();
                        }));
            }
        }

        public void EventHandleEnable()
        {
            if (_isWorking)
                return;
            GameManager.OnInputControlsChanged += GameManager_OnInputControlsChanged;
            GameManager_OnInputControlsChanged(GameManager.CurrentControlScheme);
            _isWorking = true;
        }

        public void EventHandleDisable()
        {
            if (!_isWorking)
                return;
            GameManager.OnInputControlsChanged -= GameManager_OnInputControlsChanged;
            _isWorking = false;
        }
    }
}
