using UnityEngine;

public static class RobotCommandSequenceSerializer
{
    public static void Save(RobotCommandSequence sequence, string path)
    {
        ScriptableObjectSerializer.Save(sequence, path);
    }

    public static void Load(RobotCommandSequence sequence, string path)
    {
        ScriptableObjectSerializer.Load(sequence, path);
    }
}
