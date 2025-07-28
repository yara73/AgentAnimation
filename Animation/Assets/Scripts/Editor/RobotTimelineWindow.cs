using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

public class RobotTimelineWindow : EditorWindow
{
    private const float LEFT_MARGIN = 50f;
    private RobotCommandTimeline _timeline;
    private GameObject _target;
    private Vector2 _scroll;
    private float _pixelsPerSecond = 100f;
    private int _activeIndex = -1;
    private bool _resizing = false;
    private bool _playing = false;
    private double _playStart;
    private float _previewTime = 0f;

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

    private void OnEnable()
    {
        EditorApplication.update += Update;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Update;
        StopTimeline();
    }

    private void Update()
    {
        if (!_playing) return;
        if (!EditorApplication.isPlaying)
        {
            var elapsed = (float)(EditorApplication.timeSinceStartup - _playStart);
            var duration = GetTimelineDuration();
            if (elapsed >= duration)
            {
                if (_timeline.loop)
                {
                    elapsed %= Mathf.Max(duration, 0.0001f);
                    _playStart = EditorApplication.timeSinceStartup - elapsed;
                }
                else
                {
                    StopTimeline();
                    PreviewTimeline(duration);
                    return;
                }
            }
            PreviewTimeline(elapsed);
        }
        Repaint();
    }

    private void OnGUI()
    {
        _timeline = (RobotCommandTimeline)EditorGUILayout.ObjectField("Timeline", _timeline, typeof(RobotCommandTimeline), false);
        _target = (GameObject)EditorGUILayout.ObjectField("Target", _target, typeof(GameObject), true);
        if (_timeline == null)
            return;
        _timeline.loop = EditorGUILayout.Toggle("Loop", _timeline.loop);

        _pixelsPerSecond = EditorGUILayout.Slider("Pixels Per Second", _pixelsPerSecond, 10f, 500f);

        var currentTime = _playing ? (float)(EditorApplication.timeSinceStartup - _playStart) : _previewTime;
        var maxTime = (from entry in _timeline.commands where entry?.command != null select entry.startTime + entry.command.GetDuration()).Prepend(0f).Max();

        var rect = GUILayoutUtility.GetRect(position.width - 20, 120);
        var width = Mathf.Max(rect.width, (maxTime + 5f) * _pixelsPerSecond + LEFT_MARGIN);
        var contentRect = new Rect(0, 0, width, rect.height);
        _scroll = GUI.BeginScrollView(rect, _scroll, contentRect);

        DrawTimeScale(contentRect);
        DrawCommands(currentTime);

        if (_playing)
        {
            Handles.color = Color.red;
            var x = currentTime * _pixelsPerSecond + LEFT_MARGIN;
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
            var path = EditorUtility.SaveFilePanel("Save Robot Timeline", "", "RobotTimeline.json", "json");
            if (!string.IsNullOrEmpty(path))
                RobotCommandTimelineSerializer.Save(_timeline, path);
        }
        if (GUILayout.Button("Load"))
        {
            var path = EditorUtility.OpenFilePanel("Load Robot Timeline", "", "json");
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

    private void DrawTimeScale(Rect rect)
    {
        Handles.color = Color.gray;
        var steps = Mathf.CeilToInt((rect.width - LEFT_MARGIN) / _pixelsPerSecond);
        for (var i = 0; i <= steps; i++)
        {
            var x = i * _pixelsPerSecond + LEFT_MARGIN;
            Handles.DrawLine(new Vector2(x, 0), new Vector2(x, rect.height));
            GUI.Label(new Rect(x + 2, 0, 40, 20), i.ToString());
        }
    }

    private void DrawCommands(float currentTime)
    {
        var e = Event.current;
        for (var i = 0; i < _timeline.commands.Count; i++)
        {
            var entry = _timeline.commands[i];
            if (entry?.command == null)
                continue;

            var x = entry.startTime * _pixelsPerSecond + LEFT_MARGIN;
            var w = Mathf.Max(40, entry.command.GetDuration() * _pixelsPerSecond);
            var r = new Rect(x, 20 + i * 22, w, 20);
            var resize = new Rect(r.xMax - 4, r.y, 4, r.height);

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
                        var delta = e.delta.x / _pixelsPerSecond;
                        if (_resizing)
                        {
                            var newDur = Mathf.Max(0, entry.command.GetDuration() + delta);
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

            var prev = GUI.color;
            if (_playing && currentTime >= entry.startTime && currentTime <= entry.startTime + entry.command.GetDuration())
                GUI.color = Color.yellow;
            else
                GUI.color = GetColorForCommand(entry.command);
            GUI.Box(r, entry.command.GetType().Name);
            GUI.color = prev;
        }
    }

    private static void SetCommandDuration(IRobotCommand command, float value)
    {
        RobotCommandRegistry.SetDuration(command, value);
    }

    private static Color GetColorForCommand(IRobotCommand command)
    {
        var hash = command.GetType().Name.GetHashCode();
        var hue = (hash & 0xFFFFFF) / (float)0xFFFFFF;
        return Color.HSVToRGB(hue, 0.6f, 0.8f);
    }

    private float GetTimelineDuration()
    {
        return !_timeline ? 
            0f : 
            _timeline.commands.
                Where(c => c is { command: not null }).
                    Select(entry => entry.startTime + entry.command.GetDuration()).Prepend(0f).Max();
    }

    private void ShowAddMenu()
    {
        var menu = new GenericMenu();
        RobotCommandRegistry.PopulateMenu(menu, AddCommand);
        menu.ShowAsContext();
    }

    private void AddCommand(Type type)
    {
        if (!_timeline)
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

    private void PlayTimeline()
    {
        if (!_target)
            _target = Selection.activeGameObject;
        if (!_target)
            return;
        var executor = _target.GetComponent<RobotExecutor>();
        if (!executor)
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

    private void StopTimeline()
    {
        _playing = false;
        if (!_target)
            return;
        var executor = _target.GetComponent<RobotExecutor>();
        if (!executor) return;
        if (EditorApplication.isPlaying)
            executor.Stop();
        else
            executor.Preview(0f);
    }

    private void PreviewTimeline(float time)
    {
        if (!_target)
            _target = Selection.activeGameObject;
        if (!_target)
            return;
        var executor = _target.GetComponent<RobotExecutor>();
        if (!executor)
            executor = _target.AddComponent<RobotExecutor>();
        executor.timeline = _timeline;
        executor.Stop();
        executor.Preview(time);
    }
}
