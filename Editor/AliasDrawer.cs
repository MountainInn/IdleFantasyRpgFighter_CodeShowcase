using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Alias))]
public class AliasDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty valueProp = property.FindPropertyRelative("Value");

        Rect singleLine = position;
        singleLine.height = EditorGUIUtility.singleLineHeight;

        valueProp.stringValue = Draw(singleLine, valueProp.stringValue);

        EditorGUI.EndProperty();
    }

    public static string Draw(Rect position, string oldValue)
    {
        Rect aliasRect = EditorGUI.PrefixLabel(position, new GUIContent("Alias"));

        string newValue = DrawBasic(aliasRect, oldValue);

        return newValue;
    }

    public static string DrawBasic(Rect position, string oldValue)
    {
        string newValue = EditorGUI.TextField(position, oldValue);

        newValue = StringExtension.ReplaceWhitespace(newValue, "");

        string text = AliasDictionary.instance.GetText(newValue);

        position.y += EditorGUIUtility.singleLineHeight;

        EditorGUI.LabelField(position, text);
        return newValue;
    }

    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2;
    }
}
