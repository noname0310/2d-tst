#nullable enable
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Game
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class BlurUi : MonoBehaviour
    {
        private Image? _image;
        private Material? _material;
        private Coroutine? _coroutine;
        private bool _blured;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            _image = GetComponent<Image>();
            _material = _image!.material;
            _blured = false;
        }

        public void ShowBlur()
        {
            if (_blured)
                return;
            _image!.enabled = true;
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(ShowBlurEnumerator());
        }

        private IEnumerator ShowBlurEnumerator()
        {
            var elapsedTime = 0f;
            const float duration = 0.5f;

            while (elapsedTime < duration)
            {
                yield return null;
                elapsedTime += Time.unscaledDeltaTime;

                _material!.SetFloat("BlurValue", elapsedTime / duration * 0.004f);
            }

            _blured = true;
        }

        public void HideBlur()
        {
            if (!_blured)
                return;
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(HideBlurEnumerator());
        }

        private IEnumerator HideBlurEnumerator()
        {
            var elapsedTime = 0f;
            const float duration = 0.5f;

            while (elapsedTime < duration)
            {
                yield return null;
                elapsedTime += Time.unscaledDeltaTime;

                _material!.SetFloat("BlurValue", (duration - elapsedTime) / duration * 0.004f);
            }
            _image!.enabled = false;
            _blured = false;
        }
    }
}
