using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Price))]
public class PriceDrawer : PropertyDrawer
{
    GUILayoutOption
        width = GUILayout.Width(10),
        width2 = GUILayout.Width(50),
        width3 = GUILayout.Width(100)
        ;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect r = position;
        r.width = 100;

        EditorGUI.LabelField(r, property.name);

        r.x += r.width;
        r.width = 140;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("currency"),
                                new GUIContent(""));

        r.x += r.width;
        r.width = 80;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("cost"),
                                new GUIContent(""));

        EditorGUI.EndProperty();
    }
}
