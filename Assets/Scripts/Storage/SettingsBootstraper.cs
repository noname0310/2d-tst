#nullable enable
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Assets.Scripts.Sound;
using Assets.Scripts.Storage.Model;

namespace Assets.Scripts.Storage
{
    public static class SettingsBootstraper
    {
        public static void Load(SoundManager sound, Settings settings)
        {
            Application.targetFrameRate = 60;
            sound.SfxVolume = settings.Sfx;
            sound.MusicVolume = settings.Music;
            Screen.SetResolution(settings.Resolution.Width, settings.Resolution.Hight, settings.ScreenMode);
            QualitySettings.vSyncCount = settings.Vsync ? 1 : 0;
            if (string.IsNullOrEmpty(settings.Language)) return;
            LocalizationSettings.SelectedLocale = Locale.CreateLocale(settings.Language!);
        }
    }
}
