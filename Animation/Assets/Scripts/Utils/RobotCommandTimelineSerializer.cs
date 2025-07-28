using UnityEngine;

public static class RobotCommandTimelineSerializer
{
    public static void Save(RobotCommandTimeline timeline, string path)
    {
        ScriptableObjectSerializer.Save(timeline, path);
    }

    public static void Load(RobotCommandTimeline timeline, string path)
    {
        ScriptableObjectSerializer.Load(timeline, path);
    }
}
