using System.IO;

namespace Assets.Scripts.Loader
{
    public static class ScenePathConverter
    {
        public static string GetLoadableScenePath(string path) => Path.ChangeExtension(path, null).Substring(7);
    }
}
