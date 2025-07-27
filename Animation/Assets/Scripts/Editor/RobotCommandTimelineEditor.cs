using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(RobotCommandTimeline))]
public class RobotCommandTimelineEditor : Editor
{
    ReorderableList _list;

    void OnEnable()
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
            menu.AddItem(new GUIContent("Move"), false, () => AddCommand<MoveCommand>());
            menu.AddItem(new GUIContent("Rotate"), false, () => AddCommand<RotateCommand>());
            menu.AddItem(new GUIContent("Change Color"), false, () => AddCommand<ColorCommand>());
            menu.AddItem(new GUIContent("Wait"), false, () => AddCommand<WaitCommand>());
            menu.ShowAsContext();
        };
    }

    void AddCommand<T>() where T : RobotCommand, new()
    {
        serializedObject.Update();
        var prop = serializedObject.FindProperty("commands");
        int index = prop.arraySize;
        prop.InsertArrayElementAtIndex(index);
        var element = prop.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("startTime").floatValue = 0f;
        element.FindPropertyRelative("command").managedReferenceValue = new T();
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _list.DoLayoutList();
        if (GUILayout.Button("Open Timeline Window"))
        {
            RobotTimelineWindow.Open((RobotCommandTimeline)target);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
