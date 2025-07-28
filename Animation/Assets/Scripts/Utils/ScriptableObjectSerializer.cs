using System.IO;
using UnityEngine;

public static class ScriptableObjectSerializer
{
    public static void Save<T>(T obj, string path) where T : ScriptableObject
    {
        if (!obj || string.IsNullOrEmpty(path))
            return;
        var json = JsonUtility.ToJson(obj, true);
        File.WriteAllText(path, json);
    }

    public static void Load<T>(T obj, string path) where T : ScriptableObject
    {
        if (!obj || string.IsNullOrEmpty(path) || !File.Exists(path))
            return;
        var json = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(json, obj);
    }
}
