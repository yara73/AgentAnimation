using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RobotCommand), true)]
public class RobotCommandDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var type = property.managedReferenceFullTypename.Split(' ').Last();
        label = new GUIContent(type.Substring(type.LastIndexOf('.') + 1));
        EditorGUI.PropertyField(position, property, label, true);
    }
}
