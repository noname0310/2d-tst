#nullable enable
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Loader;

namespace Assets.Scripts.UI.Title
{
    [DisallowMultipleComponent]
    public class TitleMenuUi : MonoBehaviour
    {
        public Button? NewGameButton;
        public Button? ContinueButton;
        public Button? SettingsButton;
        public Button? QuitButton;
        public Button? SettingsCloseButton;
        public GameObject? TitleMenuPanel;
        public GameObject? SettingsPanel;

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            NewGameButton!.onClick.AddListener(OnNewGameButtonClick);
            ContinueButton!.onClick.AddListener(OnContinueButtonClick);
            SettingsButton!.onClick.AddListener(OnSettingsButtonClick);
            QuitButton!.onClick.AddListener(OnQuitButtonClick);
            SettingsCloseButton!.onClick.AddListener(OnSettingsCloseButtonClick);
            GameManager.Ui!.OnPause += Ui_OnPause;
            ContinueButton.interactable = GameManager.Storage!.IsSaveDataExist;
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            NewGameButton!.onClick.RemoveListener(OnNewGameButtonClick);
            ContinueButton!.onClick.RemoveListener(OnContinueButtonClick);
            SettingsButton!.onClick.RemoveListener(OnSettingsButtonClick);
            QuitButton!.onClick.RemoveListener(OnQuitButtonClick);
            if (GameManager.Ui != null) GameManager.Ui.OnPause -= Ui_OnPause;
        }

        private void OnNewGameButtonClick()
        {
            NewGameButton!.interactable = false;
            ContinueButton!.interactable = false;
            SettingsButton!.interactable = false;
            QuitButton!.interactable = false;
            GameManager.Storage!.InitGameData();
            SceneLoader.LoadGame();
        }

        private void OnContinueButtonClick()
        {
            NewGameButton!.interactable = false;
            ContinueButton!.interactable = false;
            SettingsButton!.interactable = false;
            QuitButton!.interactable = false;
            GameManager.Storage!.LoadGameData();
            SceneLoader.LoadGame();
        }

        private void OnSettingsButtonClick()
        {
            TitleMenuPanel!.SetActive(false);
            SettingsPanel!.SetActive(true);
        }

        private static void OnQuitButtonClick() => Application.Quit();

        private void OnSettingsCloseButtonClick()
        {
            TitleMenuPanel!.SetActive(true);
            SettingsPanel!.SetActive(false);
            GameManager.Storage!.SaveSettings();
        }

        private void Ui_OnPause() => OnSettingsCloseButtonClick();
    }
}
