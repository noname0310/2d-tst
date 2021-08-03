#nullable enable
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [DisallowMultipleComponent]
    public class SettingsUi : MonoBehaviour
    {
        public Slider? SfxSlider;
        public Slider? MusicSlider;
        public Dropdown? ScreenModeDropdown;
        public Dropdown? ResolutionDropdown;
        public Dropdown? VsyncDropdown;
        public Dropdown? LocalizationDropdown;
        public GameObject? BlockGraphicsOption;

        private FullScreenMode _screenMode;
        private int _resolution;

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            SfxSlider!.SetValueWithoutNotify(Mathf.Pow(10.0f, GameManager.Sound!.SfxVolume / 20.0f));
            SfxSlider.onValueChanged.AddListener(OnSfxSliderValueChanged);

            MusicSlider!.SetValueWithoutNotify(Mathf.Pow(10.0f, GameManager.Sound!.MusicVolume / 20.0f));
            MusicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);

            if (SystemInfo.deviceType != DeviceType.Handheld)
            {
                switch (Screen.fullScreenMode)
                {
                    case FullScreenMode.ExclusiveFullScreen:
                        ScreenModeDropdown!.SetValueWithoutNotify(0);
                        break;
                    case FullScreenMode.FullScreenWindow:
                        ScreenModeDropdown!.SetValueWithoutNotify(1);
                        break;
                    case FullScreenMode.MaximizedWindow:
                        ScreenModeDropdown!.SetValueWithoutNotify(2);
                        break;
                    case FullScreenMode.Windowed:
                        ScreenModeDropdown!.SetValueWithoutNotify(3);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _screenMode = Screen.fullScreenMode;
                ScreenModeDropdown.onValueChanged.AddListener(OnScreenModeDropdownValueChanged);

                ResolutionDropdown!.ClearOptions();
                var resolutions = Screen.resolutions.Select(item => $"{item.width}x{item.height}").ToList();
                ResolutionDropdown.AddOptions(resolutions);
                var currentResolution = $"{Screen.currentResolution.width}x{Screen.currentResolution.height}";
                var resolutionFound = false;
                for (var i = 0; i < resolutions.Count; i++)
                {
                    var item = resolutions[i];
                    if (item != currentResolution) continue;
                    ResolutionDropdown.SetValueWithoutNotify(i);
                    _resolution = i;
                    resolutionFound = true;
                    break;
                }

                if (!resolutionFound)
                {
                    _resolution = 0;
                    ResolutionDropdown.SetValueWithoutNotify(0);
                }

                ResolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownValueChanged);

                VsyncDropdown!.SetValueWithoutNotify(QualitySettings.vSyncCount);
                VsyncDropdown.onValueChanged.AddListener(OnVsyncDropdownValueChanged);
                Destroy(BlockGraphicsOption);
            }

            //if (LocalizationSettings.SelectedLocale == null || LocalizationSettings.SelectedLocale.Formatter == null)
            //    LocalizationSettings.SelectedLocale = Locale.CreateLocale(CultureInfo.CurrentCulture);

            switch (LocalizationSettings.SelectedLocale.Formatter.ToString())
            {
                case "en":
                    LocalizationDropdown!.SetValueWithoutNotify(0);
                    break;
                case "ko-KR":
                    LocalizationDropdown!.SetValueWithoutNotify(1);
                    break;
            }

            LocalizationDropdown!.onValueChanged.AddListener(OnLocalizationDropdownValueChanged);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            SfxSlider!.onValueChanged.RemoveListener(OnSfxSliderValueChanged);
            MusicSlider!.onValueChanged.RemoveListener(OnMusicSliderValueChanged);
            ScreenModeDropdown!.onValueChanged.RemoveListener(OnScreenModeDropdownValueChanged);
            ResolutionDropdown!.onValueChanged.RemoveListener(OnResolutionDropdownValueChanged);
            VsyncDropdown!.onValueChanged.RemoveListener(OnVsyncDropdownValueChanged);
            LocalizationDropdown!.onValueChanged.RemoveListener(OnLocalizationDropdownValueChanged);
        }

        private static void OnSfxSliderValueChanged(float value)
        {
            GameManager.Sound!.SfxVolume = Mathf.Log10(value) * 20;
            GameManager.Storage!.Settings!.Sfx = GameManager.Sound!.SfxVolume;
        }

        private static void OnMusicSliderValueChanged(float value)
        {
            GameManager.Sound!.MusicVolume = Mathf.Log10(value) * 20;
            GameManager.Storage!.Settings!.Music = GameManager.Sound!.MusicVolume;
        }

        private void OnScreenModeDropdownValueChanged(int value)
        {
            _screenMode = (FullScreenMode)value;
            var resolution = Screen.resolutions[_resolution];
            Screen.SetResolution(resolution.width, resolution.height, _screenMode);
            GameManager.Storage!.Settings!.ScreenMode = _screenMode;
        }

        private void OnResolutionDropdownValueChanged(int value)
        {
            _resolution = value;
            var resolution = Screen.resolutions[_resolution];
            Screen.SetResolution(resolution.width, resolution.height, _screenMode);
            GameManager.Storage!.Settings!.Resolution.Width = resolution.width;
            GameManager.Storage.Settings.Resolution.Hight = resolution.height;
        }

        private static void OnVsyncDropdownValueChanged(int value)
        {
            QualitySettings.vSyncCount = value;
            GameManager.Storage!.Settings!.Vsync = QualitySettings.vSyncCount == 1;
        }

        private static void OnLocalizationDropdownValueChanged(int value)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[value];
            GameManager.Storage!.Settings!.Language = LocalizationSettings.SelectedLocale.Formatter.ToString();
        }
    }
}
