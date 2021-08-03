#nullable enable
using UnityEngine;

namespace Assets.Scripts.Render
{
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public class BlurRenderer : MonoBehaviour
    {
        public Material? BlurMaterial;
        private Camera? _blurCamera;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            _blurCamera = GetComponent<Camera>();
            if (_blurCamera.targetTexture != null)
                _blurCamera.targetTexture.Release();
            _blurCamera.targetTexture =
                new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, 1);
            BlurMaterial!.SetTexture("_RenTex", _blurCamera.targetTexture);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            if (_blurCamera!.targetTexture == null) return;
            var renderTexture = _blurCamera.targetTexture;
            _blurCamera.targetTexture = null;
            DestroyImmediate(renderTexture);
        }
    }
}
