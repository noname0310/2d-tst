#nullable enable
using System;
using UnityEngine;
using Assets.Scripts.UI;

namespace Assets.Scripts.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeAnimationController : MonoBehaviour
    {
        public float FadeInDuration = 0.5f;
        public float FadeOutDuration = 0.5f;
        private Coroutine? _coroutine;
        private CanvasGroup? _canvasGroup;
        private Action? _onAnimationComplete;

        // ReSharper disable once UnusedMember.Local
        private void Awake() =>
            _canvasGroup = GetComponent<CanvasGroup>();

        // ReSharper disable once UnusedMember.Local
        private void OnEnable()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine =
                StartCoroutine(CoroutineAnimation.FadeCanvasGroupAlpha(_canvasGroup!, true, FadeInDuration,
                    () =>
                    {
                        _onAnimationComplete?.Invoke();
                        _onAnimationComplete = null;
                    }));
        }

        public void Enable(Action? onAnimationComplete)
        {
            if (gameObject.activeSelf)
                return;
            gameObject.SetActive(true);
            _onAnimationComplete = onAnimationComplete;
        }

        public void Disable(Action? onAnimationComplete)
        {
            if (!gameObject.activeSelf)
                return;
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(CoroutineAnimation.FadeCanvasGroupAlpha(_canvasGroup!, false, FadeOutDuration,
                () =>
                {
                    gameObject.SetActive(false); 
                    onAnimationComplete?.Invoke();
                }));
        }

        public void DisableImmidate() => gameObject.SetActive(false);
    }
}
