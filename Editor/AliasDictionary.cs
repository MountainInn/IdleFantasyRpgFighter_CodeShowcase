using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[FilePathAttribute("Assets/AliasDictionary.asset", FilePathAttribute.Location.ProjectFolder)]
[CreateAssetMenuAttribute(fileName = "Alias Dictionary", menuName = "S Singleton/AliasDictionary")]
public class AliasDictionary : ScriptableSingleton<AliasDictionary>, ISerializationCallbackReceiver
{
    [SerializeField] public List<string> aliases, texts;

    public Dictionary<string, string> dict;


    public void Insert(string alias, string text)
    {
        while (aliases.Contains(alias))
        {
            int id = aliases.IndexOf(alias);

            aliases.RemoveAt(id);
            texts.RemoveAt(id);
        }

        aliases.Add(alias);
        texts.Add(text);
    }

    public void Save()
    {
        Save(true);
        EditorUtility.RequestScriptReload();
    }

    public string GetText(Alias alias)
    {
        if (dict is null)
        {
            ((ISerializationCallbackReceiver)this).OnAfterDeserialize();
        }

        if (dict.TryGetValue(alias, out string value))
        {
            return value;
        }

        return "[Alias Not Found]";
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        dict ??= new();

        foreach (var (k, v) in IEnumerableExt .Zip(aliases, texts))
        {
            dict.TryAdd(k, v);
        }
    }
}
