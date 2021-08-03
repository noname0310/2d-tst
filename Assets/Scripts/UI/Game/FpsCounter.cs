#nullable enable
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Game
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class FpsCounter : MonoBehaviour
    {
        private Text? _text;
        private static string[]? _fpsStringPool;
        private const int MaxCount = 160;
        private const int UpdateTickMax = 60;
        private int _updateTick;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            _text = GetComponent<Text>();

            if (_fpsStringPool != null) return;
            _fpsStringPool = new string[MaxCount];
            for (var i = 0; i < _fpsStringPool.Length; i++) _fpsStringPool[i] = $"{i} FPS";
        }

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            if (_updateTick != UpdateTickMax)
            {
                _updateTick += 1;
                return;
            }

            var fps = 1 / Time.unscaledDeltaTime;
            if (fps < 0)
                fps = 0;
            if (MaxCount - 1 < fps)
                fps = MaxCount - 1;
            _text!.text = _fpsStringPool![(int)fps];
            _updateTick = 0;
        }
    }
}
