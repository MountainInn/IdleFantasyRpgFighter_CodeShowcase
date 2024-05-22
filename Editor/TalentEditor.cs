using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Talent), true)]
public class TalentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        serializedObject.Update();

        SerializedProperty maxLevelProp = serializedObject.FindProperty("maxLevel");

        EditorGUILayout.PropertyField(maxLevelProp);

        int targetLength = maxLevelProp.intValue + 1;

        SerializedProperty item = serializedObject.GetIterator();

        bool hasNext = item.Next(true);

        Dictionary<string, List<SerializedProperty>> statLists = new();

        while (hasNext = item.Next(false))
        {
            if (item.isArray && item.type == "Field")
            {
                int diff = (targetLength - item.arraySize);

                Mathf
                    .Abs(diff)
                    .ForLoop(i =>
                    {
                        int lastIndex = Mathf.Max(0, item.arraySize - 1);

                        if (diff > 0)
                        {
                            item.InsertArrayElementAtIndex(lastIndex);
                        }
                        else if (diff < 0)
                        {
                            item.DeleteArrayElementAtIndex(lastIndex);
                        }
                    });

                statLists.Add(item.name, new());

                targetLength
                    .ForLoop(i =>
                    {
                        statLists[item.name].Add(item
                                                 .GetArrayElementAtIndex(i)
                                                 .FindPropertyRelative("intValue"));
                    });
            }
        }

        EditorGUILayout.BeginHorizontal();

        var width = GUILayout.Width(100);

        EditorGUILayout.LabelField("Level", width);

        foreach (var (statName, stats) in statLists)
        {
            EditorGUILayout.LabelField(statName, width);
        }

        EditorGUILayout.EndHorizontal();

        targetLength
            .ForLoop(i =>
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(i.ToString(), width);

                foreach (var (statName, stats) in statLists)
                {
                    EditorGUILayout.PropertyField(stats[i], new GUIContent(""), width);
                }

                EditorGUILayout.EndHorizontal();
            });

        serializedObject.ApplyModifiedProperties();
    }
}
