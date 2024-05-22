
using UnityEditor;

public static class GameSettingsMenu
{
    private const string cheatsPath = "Assets/Resources/Cheats.asset";

    [MenuItem("☸ Configs/Global Game Settings")]
    static public void OpenGlobalGameSettings()
    {
        Selection.objects = new[] {
            EnsureLoadAssetAtPath<GameSettings>("Assets/Resources/GameSettings.asset")
        };
    }


    [MenuItem("☸ Configs/Cheats")]
    static public void OpenCheats()
    {
        Selection.objects = new[] {
            EnsureLoadAssetAtPath<Cheats>("Assets/Resources/Cheats.asset")
        };
    }


    static public T EnsureLoadAssetAtPath<T>(string path)
        where T : UnityEngine.Object, new()
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset == null)
            {
                AssetDatabase.CreateAsset(new T(), path);
            }

            asset = AssetDatabase.LoadAssetAtPath<T>(path);

            return asset;
        }
}
