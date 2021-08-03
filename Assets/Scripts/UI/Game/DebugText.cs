#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Game
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class DebugText : MonoBehaviour
    {
        public static string Text
        {
            get => _text!;
            set
            {
                _text = value;
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var index = 0; index < _debugTexts!.Count; index++)
                    _debugTexts[index].text = _text;
            }
        }

        private static string? _text;
        private static List<Text>? _debugTexts;
        private Text? _textUi;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            _textUi = GetComponent<Text>();
            _textUi.text = _text!;
            _debugTexts ??= new List<Text>();
            _debugTexts.Add(_textUi);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy() => _debugTexts!.Remove(_textUi!);
    }
}
