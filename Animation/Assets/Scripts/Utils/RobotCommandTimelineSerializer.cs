using System.IO;
using UnityEngine;

public static class RobotCommandTimelineSerializer
{
    public static void Save(RobotCommandTimeline timeline, string path)
    {
        if (timeline == null || string.IsNullOrEmpty(path))
            return;
        var json = JsonUtility.ToJson(timeline, true);
        File.WriteAllText(path, json);
    }

    public static void Load(RobotCommandTimeline timeline, string path)
    {
        if (timeline == null || string.IsNullOrEmpty(path) || !File.Exists(path))
            return;
        var json = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(json, timeline);
    }
}
