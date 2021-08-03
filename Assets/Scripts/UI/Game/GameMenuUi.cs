#nullable enable
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using Assets.Scripts.Loader;
using Assets.Scripts.Controller;

namespace Assets.Scripts.UI.Game
{
    [DisallowMultipleComponent]
    public class GameMenuUi : MonoBehaviour
    {
        public Button? SaveButton;
        public Button? SettingsButton;
        public Button? ExitButton;
        public Text? PopupText;
        public GameUiController? GameUiController;

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            SaveButton!.onClick.AddListener(OnSaveButtonClick);
            SettingsButton!.onClick.AddListener(OnSettingsButtonClick);
            ExitButton!.onClick.AddListener(OnExitButtonClick);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            SaveButton!.onClick.RemoveListener(OnSaveButtonClick);
            SettingsButton!.onClick.RemoveListener(OnSettingsButtonClick);
            ExitButton!.onClick.RemoveListener(OnExitButtonClick);
        }

        private void OnSaveButtonClick()
        {
            GameManager.Storage!.SaveGameData();
            
            var asyncOperationHandle = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("UI", "Saved");
            if (asyncOperationHandle.IsDone)
                ShowPopupText(asyncOperationHandle.Result);
            else
                asyncOperationHandle.Completed += handle =>
                    ShowPopupText(handle.Result); 
            
            void ShowPopupText(string message)
            {
                PopupText!.text = message;
                GameUiController!.MenuController!.ShowPopupMessage();
            }
        }

        private void OnSettingsButtonClick() => GameUiController!.MenuController!.OpenSettings();

        private static void OnExitButtonClick() => SceneLoader.LoadTitle();
    }
}
