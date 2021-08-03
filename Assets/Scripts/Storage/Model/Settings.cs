#nullable enable
using System;
using UnityEngine;

namespace Assets.Scripts.Storage.Model
{
    [Serializable]
    public class Settings
    {
        public float Sfx;
        public float Music;
        public FullScreenMode ScreenMode;
        public ResolutionData Resolution;
        public bool Vsync;
        /// <summary>
        /// this member need to be lazy init
        /// </summary>
        public string? Language;

        public static Settings BuildDefault() =>
            new Settings()
            {
                Sfx = 0f,
                Music = 0f,
                ScreenMode = Screen.fullScreenMode,
                Resolution = new ResolutionData
                {
                    Width = Screen.currentResolution.width,
                    Hight = Screen.currentResolution.height
                },
                Vsync = QualitySettings.vSyncCount == 1,
                Language = null
            };

        [Serializable]
        public struct ResolutionData
        {
            public int Width;
            public int Hight;
        }
    }
}
