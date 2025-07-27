using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RobotTimedCommand))]
public class RobotTimedCommandDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var commandProp = property.FindPropertyRelative("command");
        return EditorGUI.GetPropertyHeight(commandProp) + EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var startProp = property.FindPropertyRelative("startTime");
        var commandProp = property.FindPropertyRelative("command");

        Rect startRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        startProp.floatValue = EditorGUI.FloatField(startRect, "Start Time", startProp.floatValue);

        Rect commandRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(commandRect, commandProp, GUIContent.none, true);
    }
}
