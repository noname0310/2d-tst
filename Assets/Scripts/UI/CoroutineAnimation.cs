#nullable enable
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public static class CoroutineAnimation
    {
        public static IEnumerator FadeCanvasGroupAlpha(CanvasGroup canvasGroup, bool isFadeIn, float duration, Action? onComplete)
        {
            canvasGroup.alpha = isFadeIn ? 0 : 1;
            var elapsedTime = 0f;
            while (elapsedTime <= duration)
            {
                yield return null;
                elapsedTime += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, elapsedTime / duration);
            }
            onComplete?.Invoke();
        }

        public static IEnumerator RectTransformMoveLerp(RectTransform rectTransform, Rect rect, float duration, Action? onComplete)
        {
            var elapsedTime = 0f;
            while (elapsedTime <= duration)
            {
                yield return null;
                elapsedTime += Time.unscaledDeltaTime;
                var lerpTime = elapsedTime / duration;
                rectTransform.offsetMax = Vector2.Lerp(rectTransform.offsetMax, rect.max, lerpTime);
                rectTransform.offsetMin = Vector2.Lerp(rectTransform.offsetMin, rect.min, lerpTime);
            }
            onComplete?.Invoke();
        }

        public static IEnumerator FadeAudio(AudioSource audioSource, bool isFadeIn, float duration, Action? onComplete)
        {
            audioSource.volume = isFadeIn ? 0 : 1;
            var elapsedTime = 0f;
            while (elapsedTime <= duration)
            {
                yield return null;
                elapsedTime += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, elapsedTime / duration);
            }
            onComplete?.Invoke();
        }
    }
}
