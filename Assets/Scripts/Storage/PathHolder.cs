namespace Assets.Scripts.Storage
{
    internal class PathHolder
    {
        public string SettingsPath { get; }
        public string SaveDataPath { get; }
        public PathHolder(string settingsPath, string saveDataPath) =>
            (SettingsPath, SaveDataPath) = (settingsPath, saveDataPath);
    }
}
