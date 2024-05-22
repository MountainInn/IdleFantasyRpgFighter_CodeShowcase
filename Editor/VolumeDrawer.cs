
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Volume))]
public class VolumeDrawer : PropertyDrawer
{
    GUILayoutOption
        width = GUILayout.Width(10),
        width2 = GUILayout.Width(50),
        width3 = GUILayout.Width(100)
        ;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField(property.name);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_current"),
                                          new GUIContent(""),
                                          width2);

            EditorGUILayout.LabelField("/", width);

            EditorGUILayout.PropertyField(property.FindPropertyRelative("_maximum"),
                                          new GUIContent(""),
                                          width2);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.EndProperty();
    }
}
