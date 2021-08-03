#nullable enable
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Loader;

namespace Assets.Scripts.UI.Credit
{
    [RequireComponent(typeof(Button))]
    public class CreditSkipUi : MonoBehaviour
    {
        private Button? _button;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            if (!ComponentReferenceHolder.CreditFromTitle)
            {
                Destroy(gameObject);
                return;
            }

            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }

        // ReSharper disable once UnusedMember.Local
        private void Update() => _button!.Select();

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy() => _button?.onClick.RemoveListener(OnButtonClicked);

        private static void OnButtonClicked() => SceneLoader.LoadTitle();
    }
}
