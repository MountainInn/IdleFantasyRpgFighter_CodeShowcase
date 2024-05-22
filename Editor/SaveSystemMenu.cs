using UnityEditor;
using UnityEngine;

public static class SaveSystemMenu
{
    [MenuItem("Save System/Save")]
    static public void Save()
    {
        GameObject
            .FindObjectOfType<SaveSystem>()
            .Save();
    }
}
