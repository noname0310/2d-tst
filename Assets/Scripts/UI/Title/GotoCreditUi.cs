#nullable enable
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Loader;

namespace Assets.Scripts.UI.Title
{
    [RequireComponent(typeof(Button))]
    public class GotoCreditUi : MonoBehaviour
    {
        private Button? _button;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy() => _button!.onClick.RemoveListener(OnButtonClicked);

        private static void OnButtonClicked() => SceneLoader.LoadCredit();
    }
}
