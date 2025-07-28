using UnityEditor;
using UnityEngine;
using System;

public class RobotTimelineWindow : EditorWindow
{
    const float LEFT_MARGIN = 50f;
    RobotCommandTimeline _timeline;
    GameObject _target;
    Vector2 _scroll;
    float _pixelsPerSecond = 100f;
    int _activeIndex = -1;
    bool _resizing = false;
    bool _playing = false;
    double _playStart;
    float _previewTime = 0f;

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

    void OnEnable()
    {
        EditorApplication.update += Update;
    }

    void OnDisable()
    {
        EditorApplication.update -= Update;
        StopTimeline();
    }

    void Update()
    {
        if (_playing)
        {
            if (!EditorApplication.isPlaying)
                PreviewTimeline((float)(EditorApplication.timeSinceStartup - _playStart));
            Repaint();
        }
    }

    void OnGUI()
    {
        _timeline = (RobotCommandTimeline)EditorGUILayout.ObjectField("Timeline", _timeline, typeof(RobotCommandTimeline), false);
        _target = (GameObject)EditorGUILayout.ObjectField("Target", _target, typeof(GameObject), true);
        if (_timeline == null)
            return;
        _timeline.loop = EditorGUILayout.Toggle("Loop", _timeline.loop);

        _pixelsPerSecond = EditorGUILayout.Slider("Pixels Per Second", _pixelsPerSecond, 10f, 500f);

        float maxTime = 0f;
        float currentTime = _playing ? (float)(EditorApplication.timeSinceStartup - _playStart) : _previewTime;
        foreach (var entry in _timeline.commands)
        {
            if (entry == null || entry.command == null)
                continue;
            float end = entry.startTime + entry.command.GetDuration();
            if (end > maxTime)
                maxTime = end;
        }

        Rect rect = GUILayoutUtility.GetRect(position.width - 20, 100);
        float width = Mathf.Max(rect.width, (maxTime + 5f) * _pixelsPerSecond + LEFT_MARGIN);
        Rect contentRect = new Rect(0, 0, width, rect.height);
        _scroll = GUI.BeginScrollView(rect, _scroll, contentRect);

        DrawTimeScale(contentRect);
        DrawCommands(currentTime);

        if (_playing)
        {
            Handles.color = Color.red;
            float x = currentTime * _pixelsPerSecond + LEFT_MARGIN;
            Handles.DrawLine(new Vector2(x, 0), new Vector2(x, contentRect.height));
        }

        GUI.EndScrollView();

        GUILayout.BeginHorizontal();
        if (!_playing)
        {
            if (GUILayout.Button("Play"))
            {
                PlayTimeline();
            }
        }
        else
        {
            if (GUILayout.Button("Stop"))
            {
                StopTimeline();
            }
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

        _previewTime = EditorGUILayout.FloatField("Preview Time", _previewTime);
        if (GUILayout.Button("Preview"))
        {
            PreviewTimeline(_previewTime);
        }
        GUILayout.Label($"Time: {currentTime:F2}s");

        GUILayout.Space(5);
        if (GUILayout.Button("Add State"))
        {
            ShowAddMenu();
        }
    }

    void DrawTimeScale(Rect rect)
    {
        Handles.color = Color.gray;
        int steps = Mathf.CeilToInt((rect.width - LEFT_MARGIN) / _pixelsPerSecond);
        for (int i = 0; i <= steps; i++)
        {
            float x = i * _pixelsPerSecond + LEFT_MARGIN;
            Handles.DrawLine(new Vector2(x, 0), new Vector2(x, rect.height));
            GUI.Label(new Rect(x + 2, 0, 40, 20), i.ToString());
        }
    }

    void DrawCommands(float currentTime)
    {
        Event e = Event.current;
        for (int i = 0; i < _timeline.commands.Count; i++)
        {
            var entry = _timeline.commands[i];
            if (entry == null || entry.command == null)
                continue;

            float x = entry.startTime * _pixelsPerSecond + LEFT_MARGIN;
            float w = Mathf.Max(40, entry.command.GetDuration() * _pixelsPerSecond);
            Rect r = new Rect(x, 20 + i * 22, w, 20);
            Rect resize = new Rect(r.xMax - 4, r.y, 4, r.height);

            EditorGUIUtility.AddCursorRect(resize, MouseCursor.ResizeHorizontal);
            EditorGUIUtility.AddCursorRect(r, MouseCursor.MoveArrow);

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (resize.Contains(e.mousePosition))
                    {
                        _activeIndex = i;
                        _resizing = true;
                        e.Use();
                    }
                    else if (r.Contains(e.mousePosition))
                    {
                        _activeIndex = i;
                        _resizing = false;
                        e.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (_activeIndex == i)
                    {
                        Undo.RecordObject(_timeline, "Move Command");
                        float delta = e.delta.x / _pixelsPerSecond;
                        if (_resizing)
                        {
                            float newDur = Mathf.Max(0, entry.command.GetDuration() + delta);
                            SetCommandDuration(entry.command, newDur);
                        }
                        else
                        {
                            entry.startTime = Mathf.Max(0, entry.startTime + delta);
                        }
                        e.Use();
                        EditorUtility.SetDirty(_timeline);
                    }
                    break;
                case EventType.MouseUp:
                    if (_activeIndex == i)
                    {
                        _activeIndex = -1;
                        _resizing = false;
                        e.Use();
                    }
                    break;
            }

            Color prev = GUI.color;
            if (_playing && currentTime >= entry.startTime && currentTime <= entry.startTime + entry.command.GetDuration())
                GUI.color = Color.yellow;
            else
                GUI.color = GetColorForCommand(entry.command);
            GUI.Box(r, entry.command.GetType().Name);
            GUI.color = prev;
        }
    }

    void SetCommandDuration(RobotCommand command, float value)
    {
        RobotCommandRegistry.SetDuration(command, value);
    }

    Color GetColorForCommand(RobotCommand command)
    {
        unchecked
        {
            int hash = command.GetType().Name.GetHashCode();
            float hue = (hash & 0xFFFFFF) / (float)0xFFFFFF;
            return Color.HSVToRGB(hue, 0.6f, 0.8f);
        }
    }

    void ShowAddMenu()
    {
        var menu = new GenericMenu();
        RobotCommandRegistry.PopulateMenu(menu, type => AddCommand(type));
        menu.ShowAsContext();
    }

    void AddCommand(Type type)
    {
        if (_timeline == null)
            return;
        Undo.RecordObject(_timeline, "Add Command");
        var entry = new RobotTimedCommand
        {
            startTime = 0f,
            command = (RobotCommand)Activator.CreateInstance(type)
        };
        _timeline.commands.Add(entry);
        EditorUtility.SetDirty(_timeline);
    }

    void PlayTimeline()
    {
        if (_target == null)
            _target = Selection.activeGameObject;
        if (_target == null)
            return;
        var executor = _target.GetComponent<RobotExecutor>();
        if (executor == null)
            executor = _target.AddComponent<RobotExecutor>();
        executor.timeline = _timeline;
        executor.Stop();
        if (EditorApplication.isPlaying)
            executor.Play();
        else
            executor.Preview(0f);
        _playing = true;
        _playStart = EditorApplication.timeSinceStartup;
    }

    void StopTimeline()
    {
        _playing = false;
        if (_target == null)
            return;
        var executor = _target.GetComponent<RobotExecutor>();
        if (executor != null)
        {
            if (EditorApplication.isPlaying)
                executor.Stop();
            else
                executor.Preview(0f);
        }
    }

    void PreviewTimeline(float time)
    {
        if (_target == null)
            _target = Selection.activeGameObject;
        if (_target == null)
            return;
        var executor = _target.GetComponent<RobotExecutor>();
        if (executor == null)
            executor = _target.AddComponent<RobotExecutor>();
        executor.timeline = _timeline;
        executor.Stop();
        executor.Preview(time);
    }
}
