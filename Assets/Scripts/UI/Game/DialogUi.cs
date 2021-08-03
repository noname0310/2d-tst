#nullable enable
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Optimizer;

namespace Assets.Scripts.UI.Game
{
    [DisallowMultipleComponent]
    public class DialogUi : MonoBehaviour
    {
        public event Action? OnDialogShowing;
        public event Action? OnDialogHiding;
        public event Action? OnBranchShowing;
        public event Action? OnBranchHiding;
        public event Action? OnCenterImageShowing;
        public event Action? OnCenterImageHiding;

        public string DisplayName
        {
            get => _nameText!.text;
            set => _nameText!.text = value;
        }

        public Sprite DisplaySprite
        {
            get => _charactorImage!.sprite;
            set => _charactorImage!.sprite = value;
        }

        public Sprite DisplayCenterSprite
        {
            get => _centerImage!.sprite;
            set => _centerImage!.sprite = value;
        }

        public bool ShowingText => _coroutine != null;

        public Button.ButtonClickedEvent ClickedEvent => _button!.onClick;

        [SerializeField]
#pragma warning disable 649
        private TMP_Text? _dialogTmpText;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private GameObject? _nameTextPanel;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private GameObject? _charactorImagePanel;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private Text? _nameText;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private Image? _charactorImage;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private Button? _button;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private Text? _option1Text;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private Text? _option2Text;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private Button? _option1Button;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private Button? _option2Button;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private Image? _centerImage;
#pragma warning restore 649
        [SerializeField]
#pragma warning disable 649
        private Button? _centerImageButton;
#pragma warning restore 649

        private float _xMoveOffset;
        private bool _isDialogShowing;
        private bool _isBranchShowing;
        private bool _isCenterImageShowing;
        private bool _isCharactorImageShowing;
        private bool _isWiggleing;
        private Coroutine? _coroutine;
        private Action? _onComplete;
        private TMP_MeshInfo[]? _meshCache;
        private const int MaxVisibleCharactersDefault = 99999;

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            _isDialogShowing = false;
            _isBranchShowing = false;
            _isCenterImageShowing = false;
            _isCharactorImageShowing = true;
            _isWiggleing = false;
            _centerImageButton!.onClick.AddListener(OnCenterImageClicked);
        }

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            _xMoveOffset = _dialogTmpText!.rectTransform.offsetMin.x + _dialogTmpText.rectTransform.offsetMax.x;
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
            _centerImageButton!.onClick.RemoveListener(OnCenterImageClicked);
        }

        private void OnCenterImageClicked() => _button!.onClick.Invoke();

        private void OnTextChanged(UnityEngine.Object text)
        {
            if (text != _dialogTmpText) return;
            if (!_isWiggleing) return;
            _meshCache = _dialogTmpText.textInfo.CopyMeshInfoVertexData();
            WiggleStep();
        }

        public void ShowUi()
        {
            if (_isDialogShowing) return;
            _isDialogShowing = true;
            OnDialogShowing?.Invoke();
        }

        public void HideUi()
        {
            if (!_isDialogShowing) return;
            _isDialogShowing = false;
            OnDialogHiding?.Invoke();
        }

        public void ShowCharactorImage()
        {
            if (_isCharactorImageShowing) return;
            _charactorImagePanel!.SetActive(true);
            
            var rectTransform = _dialogTmpText!.rectTransform;
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x + _xMoveOffset, rectTransform.offsetMin.y);

            var namePanelTransform = _nameTextPanel!.transform;
            namePanelTransform.position = new Vector3(
                namePanelTransform.position.x + _xMoveOffset,
                namePanelTransform.position.y,
                namePanelTransform.position.z);
            _isCharactorImageShowing = true;
        }

        public void HideCharactorImage()
        {
            if (!_isCharactorImageShowing) return;
            var rectTransform = _dialogTmpText!.rectTransform;
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x - _xMoveOffset, rectTransform.offsetMin.y);

            var namePanelTransform = _nameTextPanel!.transform;
            namePanelTransform.position = new Vector3(
                namePanelTransform.position.x - _xMoveOffset,
                namePanelTransform.position.y,
                namePanelTransform.position.z);
           
            _charactorImagePanel!.SetActive(false);
            _isCharactorImageShowing = false;
        }

        public void ShowName() => _nameTextPanel!.SetActive(true);

        public void HideName() => _nameTextPanel!.SetActive(false);

        public void PlayText(string text, float speed, Action onComplete, TextEffect textEffect = TextEffect.None)
        {
            _dialogTmpText!.ForceMeshUpdate();
            _onComplete = onComplete;
            _dialogTmpText.maxVisibleCharacters = 0;
            _dialogTmpText.text = text;
            _coroutine = StartCoroutine(PlayTextAnimation(speed, onComplete));

            if (textEffect != TextEffect.Wiggle)
            {
                _isWiggleing = false;
                return;
            }
            _isWiggleing = true;
            StartCoroutine(TextWiggleEnumerator());
        }

        public void PlayTextSync(string text, TextEffect textEffect = TextEffect.None)
        {
            _onComplete = null;
            _dialogTmpText!.maxVisibleCharacters = MaxVisibleCharactersDefault;
            _dialogTmpText.text = text;

            if (textEffect != TextEffect.Wiggle)
            {
                _isWiggleing = false;
                return;
            }
            _isWiggleing = true;
            StartCoroutine(TextWiggleEnumerator());
        }

        public void SkipText()
        {
            if (_coroutine == null)
                return;
            StopCoroutine(_coroutine);
            _coroutine = null;
            _dialogTmpText!.maxVisibleCharacters = MaxVisibleCharactersDefault;
            _onComplete?.Invoke();
        }

        public void StopWiggle() => _isWiggleing = false;

        public void ShowBranch()
        {
            if (_isBranchShowing) return;
            _isBranchShowing = true;
            OnBranchShowing?.Invoke();
        }

        public void UpdateBranch(string option1, string option2, Action<int>? onSelected)
        {
            _option1Button!.onClick.RemoveAllListeners();
            _option2Button!.onClick.RemoveAllListeners();

            _option1Text!.text = option1;
            _option2Text!.text = option2;

            var receivedEvent = false;
            _option1Button!.onClick.AddListener(Option1Clicked);
            _option2Button!.onClick.AddListener(Option2Clicked);
            void Option1Clicked()
            {
                if (receivedEvent)
                    return;
                receivedEvent = true;
                onSelected?.Invoke(0);
                _option1Button!.onClick.RemoveListener(Option1Clicked);
                _option1Button!.onClick.RemoveListener(Option2Clicked);
            }

            void Option2Clicked()
            {
                if (receivedEvent)
                    return;
                receivedEvent = true;
                onSelected?.Invoke(1);
                _option1Button!.onClick.RemoveListener(Option1Clicked);
                _option1Button!.onClick.RemoveListener(Option2Clicked);
            }
        }

        public void HideBranch()
        {
            if (!_isBranchShowing) return;
            _isBranchShowing = false;
            OnBranchHiding?.Invoke();
        }

        public void ShowCenterImage()
        {
            if (_isCenterImageShowing) return;
            _isCenterImageShowing = true;
            OnCenterImageShowing?.Invoke();
        }

        public void HideCenterImage()
        {
            if (!_isCenterImageShowing) return;
            _isCenterImageShowing = false;
            OnCenterImageHiding?.Invoke();
        }

        private IEnumerator PlayTextAnimation(float speed, Action? onComplete)
        {
            yield return YieldInstructionCache.WaitForEndOfFrame;
            var textcount = _dialogTmpText!.textInfo.characterCount;
            var elapsedTime = 0f;
            var duration = 1 / speed * textcount;

            while (elapsedTime < duration)
            {
                yield return null;
                elapsedTime += Time.unscaledDeltaTime;
                var visableCharactors = (int) (elapsedTime / duration * textcount);
                _dialogTmpText.maxVisibleCharacters = textcount <= visableCharactors 
                    ? textcount
                    : visableCharactors;
            }
            _coroutine = null;
            onComplete?.Invoke();
        }

        private IEnumerator TextWiggleEnumerator()
        {
            yield return YieldInstructionCache.WaitForEndOfFrame;
            while (_isWiggleing)
            {
                WiggleStep();
                yield return YieldInstructionCache.WaitForEndOfFrame;
            }
        }

        private void WiggleStep()
        {
            var textinfo = _dialogTmpText!.textInfo;
            var vertices = textinfo.meshInfo[0].vertices;
            var maxVisoffset = _dialogTmpText.maxVisibleCharacters - textinfo.spaceCount;
            var vertexCharacterCount = textinfo.characterCount - textinfo.spaceCount;
            int length;
            if (maxVisoffset <= vertexCharacterCount)
            {
                length = maxVisoffset;
                length <<= 2;
            }
            else
            {
                length = vertexCharacterCount;
                length <<= 2;
            }

            for (var index = 0; index < length; index++)
                vertices[index] = 
                    _meshCache![0].vertices[index] +
                                  new Vector3(UnityEngine.Random.Range(0, 2) * 4, UnityEngine.Random.Range(0, 2) * 4, 0);
            _dialogTmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }

        public enum TextEffect
        {
            None,
            Wiggle
        }
    }
}
