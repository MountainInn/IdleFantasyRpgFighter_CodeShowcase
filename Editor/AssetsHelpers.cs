#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AssetsHelpers 
{
    private static string LogPath => Directory.GetCurrentDirectory() + "/utils/logs/";

    [MenuItem("Tools/Utils/ List all paths for meshes")]
    public static void LogAssetsPathAtCurrentScene()
   {
        int sceneCount = SceneManager.sceneCount;
        StringBuilder sb = new StringBuilder(1000);
        for (int i = 0; i < sceneCount; i++)
        {
            
            List<MeshFilter> meshes = new List<MeshFilter>();
            Scene curScene = SceneManager.GetSceneAt(i);
            var allObjects = curScene.GetRootGameObjects();
            foreach (var item in allObjects)
            {
                item.GetComponentInChildren<MeshFilter>(true);
                meshes.AddRange(item.GetComponentsInChildren<MeshFilter>(true));
            }
            sb.AppendLine($"------{curScene.name}------");

            foreach (var item in meshes)
            {
                string path = AssetDatabase.GetAssetPath(item.sharedMesh);
                sb.AppendLine($"GO: {item.gameObject.name} Mesh Path: {path}");
            }
        }
        Debug.Log(LogPath + "meshes.txt");
        Utils.FileUtils.WriteAllText(LogPath + "meshes.txt", sb.ToString());
    }

}
#endif
