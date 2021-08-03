#nullable enable
using UnityEngine;
using Assets.Scripts.Loader;

namespace Assets.Scripts.UI.Credit
{
    [RequireComponent(typeof(RectTransform))]
    public class CreditUi : MonoBehaviour
    {
        public float Speed = 80f;
        private RectTransform? _rectTransform;

        // ReSharper disable once UnusedMember.Local
        private void Start() => _rectTransform = GetComponent<RectTransform>();

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            _rectTransform!.offsetMin -= Speed * Time.deltaTime * Vector2.down;
            _rectTransform.offsetMax -= Speed * Time.deltaTime * Vector2.down;
            if (_rectTransform.offsetMin.y < 0) return;
            SceneLoader.LoadTitle();
            Destroy(this);
        }
    }
}
