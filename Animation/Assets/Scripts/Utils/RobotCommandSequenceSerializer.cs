using System.IO;
using UnityEngine;

public static class RobotCommandSequenceSerializer
{
    public static void Save(RobotCommandSequence sequence, string path)
    {
        if (sequence == null || string.IsNullOrEmpty(path))
            return;
        var json = JsonUtility.ToJson(sequence, true);
        File.WriteAllText(path, json);
    }

    public static void Load(RobotCommandSequence sequence, string path)
    {
        if (sequence == null || string.IsNullOrEmpty(path) || !File.Exists(path))
            return;
        var json = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(json, sequence);
    }
}
