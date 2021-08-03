#nullable enable
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Assets.Scripts.Storage.Model;

namespace Assets.Scripts.Storage
{
    public class LocalStorage
    {
        public event Action? OnCollectGameSaveInfo;
        public event Action? OnGameSaved;
        public Settings? Settings { get; private set; }
        public GameData? GameData { get; private set; }
        public bool IsSaveDataExist => File.Exists(_saveDataFullPath);
        private readonly PathHolder _pathHolder;
        private readonly BinaryFormatter _binaryFormatter;
        private const string SaveDataName = "data1.bin";
        private readonly string _saveDataFullPath;
        private readonly string _startStageName;

        public LocalStorage(string startStageName)
        {
            _startStageName = startStageName;
            _pathHolder = new PathHolder(
                $"{Application.persistentDataPath}/settings.json",
                $"{Application.persistentDataPath}/savedata/data");
            _binaryFormatter = new BinaryFormatter();
            _saveDataFullPath = Path.Combine(_pathHolder.SaveDataPath, SaveDataName);
            LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
        }

        ~LocalStorage() => LocalizationSettings.SelectedLocaleChanged -= LocalizationSettings_SelectedLocaleChanged;

        private void LocalizationSettings_SelectedLocaleChanged(Locale obj)
        {
            if (Settings != null) Settings.Language ??= obj.Formatter.ToString();
        }

        public void LoadSettings() =>
            Settings = File.Exists(_pathHolder.SettingsPath)
                ? JsonUtility.FromJson<Settings>(File.ReadAllText(_pathHolder.SettingsPath))
                : Settings.BuildDefault();

        public void SaveSettings() =>
            File.WriteAllText(_pathHolder.SettingsPath, JsonUtility.ToJson(Settings), Encoding.UTF8);

        public void InitGameData() => GameData = GameData.BuildDefault(_startStageName);

        public void LoadGameData()
        {
            using var fileStream = new FileStream(_saveDataFullPath, FileMode.Open);
            GameData = (GameData)_binaryFormatter.Deserialize(fileStream);
        }

        public void SaveGameData()
        {
            SceneDataSync();
            if (!Directory.Exists(_saveDataFullPath))
                Directory.CreateDirectory(_pathHolder.SaveDataPath);
            using var fileStream = new FileStream(_saveDataFullPath, FileMode.OpenOrCreate);
            _binaryFormatter.Serialize(fileStream, GameData ?? throw new ArgumentNullException());
            OnGameSaved?.Invoke();
        }

        public void SceneDataSync()
        {
            GameData!.ClearScenePositions(GameData.CurrentSceneName);
            OnCollectGameSaveInfo?.Invoke();
        }
    }
}
