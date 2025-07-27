using UnityEditor;
using UnityEngine;

public class RobotTimelineWindow : EditorWindow
{
    RobotCommandTimeline _timeline;
    Vector2 _scroll;
    float _pixelsPerSecond = 100f;

    [MenuItem("Window/Robot Timeline Editor")]
    public static void OpenWindow()
    {
        GetWindow<RobotTimelineWindow>("Robot Timeline");
    }

    public static void Open(RobotCommandTimeline timeline)
    {
        var window = GetWindow<RobotTimelineWindow>("Robot Timeline");
        window._timeline = timeline;
        window.Repaint();
    }

    void OnGUI()
    {
        _timeline = (RobotCommandTimeline)EditorGUILayout.ObjectField("Timeline", _timeline, typeof(RobotCommandTimeline), false);
        if (_timeline == null)
            return;

        _pixelsPerSecond = EditorGUILayout.Slider("Pixels Per Second", _pixelsPerSecond, 10f, 500f);

        float maxTime = 0f;
        foreach (var entry in _timeline.commands)
            if (entry != null && entry.startTime > maxTime)
                maxTime = entry.startTime;

        Rect rect = GUILayoutUtility.GetRect(position.width - 20, 100);
        float width = Mathf.Max(rect.width, maxTime * _pixelsPerSecond + 100);
        Rect contentRect = new Rect(0, 0, width, rect.height);
        _scroll = GUI.BeginScrollView(rect, _scroll, contentRect);

        DrawTimeScale(contentRect);
        DrawCommands();

        GUI.EndScrollView();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Play"))
        {
            PlayTimeline();
        }
        if (GUILayout.Button("Save"))
        {
            string path = EditorUtility.SaveFilePanel("Save Robot Timeline", "", "RobotTimeline.json", "json");
            if (!string.IsNullOrEmpty(path))
                RobotCommandTimelineSerializer.Save(_timeline, path);
        }
        if (GUILayout.Button("Load"))
        {
            string path = EditorUtility.OpenFilePanel("Load Robot Timeline", "", "json");
            if (!string.IsNullOrEmpty(path))
            {
                RobotCommandTimelineSerializer.Load(_timeline, path);
                EditorUtility.SetDirty(_timeline);
            }
        }
        GUILayout.EndHorizontal();
    }

    void DrawTimeScale(Rect rect)
    {
        Handles.color = Color.gray;
        int steps = Mathf.CeilToInt(rect.width / _pixelsPerSecond);
        for (int i = 0; i <= steps; i++)
        {
            float x = i * _pixelsPerSecond;
            Handles.DrawLine(new Vector2(x, 0), new Vector2(x, rect.height));
            GUI.Label(new Rect(x + 2, 0, 40, 20), i.ToString());
        }
    }

    void DrawCommands()
    {
        for (int i = 0; i < _timeline.commands.Count; i++)
        {
            var entry = _timeline.commands[i];
            if (entry == null || entry.command == null)
                continue;
            float x = entry.startTime * _pixelsPerSecond;
            Rect r = new Rect(x + 50, 20 + i * 22, 120, 20);
            GUI.Box(r, entry.command.GetType().Name);
        }
    }

    void PlayTimeline()
    {
        var go = Selection.activeGameObject;
        if (go == null)
            return;
        var executor = go.GetComponent<RobotExecutor>();
        if (executor == null)
            return;
        executor.timeline = _timeline;
        executor.Stop();
        executor.Play();
    }
}
