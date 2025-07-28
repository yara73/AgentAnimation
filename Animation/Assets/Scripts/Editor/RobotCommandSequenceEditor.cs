using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.IO;

[CustomEditor(typeof(RobotCommandSequence))]
public class RobotCommandSequenceEditor : Editor
{
    private ReorderableList _list;

    private void OnEnable()
    {
        _list = new ReorderableList(
            serializedObject, 
            serializedObject.FindProperty("commands"), true, true, true, true)
            {
                drawHeaderCallback = rect => GUI.Label(rect, "Commands")
            };
        
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
        var index = prop.arraySize;
        prop.InsertArrayElementAtIndex(index);
        var element = prop.GetArrayElementAtIndex(index);
        element.managedReferenceValue = Activator.CreateInstance(type);
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("loop"));
        _list.DoLayoutList();
        EditorGUILayout.Space();
        if (GUILayout.Button("Save Commands"))
        {
            var path = EditorUtility.SaveFilePanel("Save Robot Commands", "", "RobotCommands.json", "json");
            if (!string.IsNullOrEmpty(path))
            {
                RobotCommandSequenceSerializer.Save((RobotCommandSequence)target, path);
            }
        }

        if (GUILayout.Button("Load Commands"))
        {
            var path = EditorUtility.OpenFilePanel("Load Robot Commands", "", "json");
            if (!string.IsNullOrEmpty(path))
            {
                RobotCommandSequenceSerializer.Load((RobotCommandSequence)target, path);
                EditorUtility.SetDirty(target);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
