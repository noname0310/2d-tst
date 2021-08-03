#nullable enable
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Core
{
    [DisallowMultipleComponent]
    public class LoadUi : MonoBehaviour
    {
        public float Prograss
        {
            get => _prograssBar!.fillAmount;
            set => _prograssBar!.fillAmount = value;
        }

        public string Text
        {
            get => _loadingText!.text;
            set => _loadingText!.text = value;
        }

        [SerializeField]
#pragma warning disable 649
        private Image? _prograssBar;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private Text? _loadingText;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private CanvasGroup? _sceneLoaderCanvasGroup;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private GameObject? _sceneLoaderCanvas;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private GameObject? _prograssPanel;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private CanvasGroup? _shadowPanelCanvasGroup;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private GameObject? _shadowPanel;
#pragma warning restore 649

        private RectTransform? _sceneLoaderRectTransform;
        private RectTransform? _shadowPanelRectTransform;
        private Coroutine? _coroutine;
        private LoadingType _loadingType;
        private Rect _shodowCenterRect;
        private Rect _shodowUpRect;
        private Rect _shodowDownRect;
        private Rect _shodowLeftRect;
        private Rect _shodowRightRect;
        private const float PrograssBarInAnimationDuration = 0.5f;
        private const float PrograssBarOutAnimationDuration = 1.0f;
        private const float ShadowInAnimationDuration = 0.3f;
        private const float ShadowOutAnimationDuration = 1.5f;

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            _sceneLoaderRectTransform = _sceneLoaderCanvas!.GetComponent<RectTransform>();
            _shadowPanelRectTransform = _shadowPanel!.GetComponent<RectTransform>();
            _shodowCenterRect = _shadowPanelRectTransform.rect;
            _shodowUpRect = _shodowCenterRect;
            _shodowUpRect.position += new Vector2(0, _shodowCenterRect.height / 2f + _sceneLoaderRectTransform.rect.height);
            _shodowDownRect = _shodowCenterRect;
            _shodowDownRect.position -= new Vector2(0, _shodowCenterRect.height / 2f + _sceneLoaderRectTransform.rect.height);
            _shodowLeftRect = _shodowCenterRect;
            _shodowLeftRect.position -= new Vector2(_shodowCenterRect.width / 2f + _sceneLoaderRectTransform.rect.width, 0);
            _shodowRightRect = _shodowCenterRect;
            _shodowRightRect.position += new Vector2(_shodowCenterRect.width / 2f + _sceneLoaderRectTransform.rect.width, 0);
        }

        public void Show(LoadingType loadingType, Action? onComplete)
        {
            _loadingType = loadingType;
            switch (loadingType)
            {
                case LoadingType.PrograssBar:
                    Prograss = 0;
                    Text = "...";
                    _sceneLoaderCanvas!.SetActive(true);
                    _prograssPanel!.SetActive(true);
                    _shadowPanel!.SetActive(false);
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine =
                        StartCoroutine(CoroutineAnimation.FadeCanvasGroupAlpha(_sceneLoaderCanvasGroup!, true, PrograssBarInAnimationDuration,
                            () => onComplete?.Invoke()));
                    break;
                case LoadingType.UpShadow:
                    _sceneLoaderCanvasGroup!.alpha = 1.0f;
                    _shadowPanelCanvasGroup!.alpha = 1.0f;
                    _sceneLoaderCanvas!.SetActive(true);
                    _prograssPanel!.SetActive(false);
                    _shadowPanel!.SetActive(true);
                    _shadowPanelRectTransform!.offsetMax = _shodowUpRect.max;
                    _shadowPanelRectTransform.offsetMin = _shodowUpRect.min;
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine =
                        StartCoroutine(CoroutineAnimation.RectTransformMoveLerp(_shadowPanelRectTransform, _shodowCenterRect, ShadowInAnimationDuration,
                            () => onComplete?.Invoke()));
                    break;
                case LoadingType.DownShadow:
                    _sceneLoaderCanvasGroup!.alpha = 1.0f;
                    _shadowPanelCanvasGroup!.alpha = 1.0f;
                    _sceneLoaderCanvas!.SetActive(true);
                    _prograssPanel!.SetActive(false);
                    _shadowPanel!.SetActive(true);
                    _shadowPanelRectTransform!.offsetMax = _shodowDownRect.max;
                    _shadowPanelRectTransform.offsetMin = _shodowDownRect.min;
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine =
                        StartCoroutine(CoroutineAnimation.RectTransformMoveLerp(_shadowPanelRectTransform, _shodowCenterRect, ShadowInAnimationDuration,
                            () => onComplete?.Invoke()));
                    break;
                case LoadingType.LeftShadow:
                    _sceneLoaderCanvasGroup!.alpha = 1.0f;
                    _shadowPanelCanvasGroup!.alpha = 1.0f;
                    _sceneLoaderCanvas!.SetActive(true);
                    _prograssPanel!.SetActive(false);
                    _shadowPanel!.SetActive(true);
                    _shadowPanelRectTransform!.offsetMax = _shodowLeftRect.max;
                    _shadowPanelRectTransform.offsetMin = _shodowLeftRect.min;
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine =
                        StartCoroutine(CoroutineAnimation.RectTransformMoveLerp(_shadowPanelRectTransform, _shodowCenterRect, ShadowInAnimationDuration,
                            () => onComplete?.Invoke()));
                    break;
                case LoadingType.RightShadow:
                    _sceneLoaderCanvasGroup!.alpha = 1.0f;
                    _shadowPanelCanvasGroup!.alpha = 1.0f;
                    _sceneLoaderCanvas!.SetActive(true);
                    _prograssPanel!.SetActive(false);
                    _shadowPanel!.SetActive(true);
                    _shadowPanelRectTransform!.offsetMax = _shodowRightRect.max;
                    _shadowPanelRectTransform.offsetMin = _shodowRightRect.min;
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine =
                        StartCoroutine(CoroutineAnimation.RectTransformMoveLerp(_shadowPanelRectTransform, _shodowCenterRect, ShadowInAnimationDuration,
                            () => onComplete?.Invoke()));
                    break;
                case LoadingType.CenterShadow:
                    _sceneLoaderCanvasGroup!.alpha = 1.0f;
                    _shadowPanelCanvasGroup!.alpha = 0f;
                    _sceneLoaderCanvas!.SetActive(true);
                    _prograssPanel!.SetActive(false);
                    _shadowPanel!.SetActive(true);
                    _shadowPanelRectTransform!.offsetMax = _shodowCenterRect.max;
                    _shadowPanelRectTransform!.offsetMin = _shodowCenterRect.min;
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine =
                        StartCoroutine(CoroutineAnimation.FadeCanvasGroupAlpha(_shadowPanelCanvasGroup!, true, ShadowInAnimationDuration,
                            () => onComplete?.Invoke()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(loadingType), loadingType, null);
            }
        }

        public void Hide()
        {
            switch (_loadingType)
            {
                case LoadingType.PrograssBar:
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine = StartCoroutine(CoroutineAnimation.FadeCanvasGroupAlpha(_sceneLoaderCanvasGroup!, false, PrograssBarOutAnimationDuration,
                        () => _sceneLoaderCanvas!.SetActive(false)));
                    break;
                case LoadingType.UpShadow:
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine = StartCoroutine(CoroutineAnimation.RectTransformMoveLerp(_shadowPanelRectTransform!, _shodowDownRect, ShadowOutAnimationDuration,
                        () => _sceneLoaderCanvas!.SetActive(false)));
                    break;
                case LoadingType.DownShadow:
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine = StartCoroutine(CoroutineAnimation.RectTransformMoveLerp(_shadowPanelRectTransform!, _shodowUpRect, ShadowOutAnimationDuration,
                        () => _sceneLoaderCanvas!.SetActive(false)));
                    break;
                case LoadingType.LeftShadow:
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine = StartCoroutine(CoroutineAnimation.RectTransformMoveLerp(_shadowPanelRectTransform!, _shodowRightRect, ShadowOutAnimationDuration,
                        () => _sceneLoaderCanvas!.SetActive(false)));
                    break;
                case LoadingType.RightShadow:
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine = StartCoroutine(CoroutineAnimation.RectTransformMoveLerp(_shadowPanelRectTransform!, _shodowLeftRect, ShadowOutAnimationDuration,
                        () => _sceneLoaderCanvas!.SetActive(false)));
                    break;
                case LoadingType.CenterShadow:
                    if (_coroutine != null)
                        StopCoroutine(_coroutine);
                    _coroutine = StartCoroutine(CoroutineAnimation.FadeCanvasGroupAlpha(_shadowPanelCanvasGroup!, false, ShadowOutAnimationDuration,
                        () => _sceneLoaderCanvas!.SetActive(false)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public enum LoadingType
        {
            PrograssBar,
            UpShadow,
            DownShadow,
            LeftShadow,
            RightShadow,
            CenterShadow
        }
    }
}
