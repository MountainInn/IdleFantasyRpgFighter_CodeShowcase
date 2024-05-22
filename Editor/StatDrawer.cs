using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Stat), true)]
public class StatDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.width /= 2;

        EditorGUI.PropertyField(position,
                                property.FindPropertyRelative("baseValue"),
                                label);

        position.x += position.xMax + 10;

        EditorGUI.PropertyField(position,
                                property
                                .FindPropertyRelative("result"),
                                new GUIContent("Result"));
    }
}
