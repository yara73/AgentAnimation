using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;

[CustomEditor(typeof(RobotCommandTimeline))]
public class RobotCommandTimelineEditor : Editor
{
    private ReorderableList _list;

    private void OnEnable()
    {
        _list = new ReorderableList(serializedObject, serializedObject.FindProperty("commands"), true, true, true, true);
        _list.drawHeaderCallback = rect => GUI.Label(rect, "Timeline Commands");
        _list.elementHeightCallback = index =>
        {
            var element = _list.serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element);
        };
        _list.drawElementCallback = (rect, index, active, focused) =>
        {
            var element = _list.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none, true);
        };
        _list.onAddDropdownCallback = (rect, list) =>
        {
            var menu = new GenericMenu();
            RobotCommandRegistry.PopulateMenu(menu, type => AddCommand(type));
            menu.ShowAsContext();
        };
    }

    private void AddCommand(Type type)
    {
        serializedObject.Update();
        var prop = serializedObject.FindProperty("commands");
        int index = prop.arraySize;
        prop.InsertArrayElementAtIndex(index);
        var element = prop.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("startTime").floatValue = 0f;
        element.FindPropertyRelative("command").managedReferenceValue = Activator.CreateInstance(type);
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("loop"));
        _list.DoLayoutList();
        if (GUILayout.Button("Open Timeline Window"))
        {
            RobotTimelineWindow.Open((RobotCommandTimeline)target);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
