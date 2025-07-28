using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class RobotCommandInfo
{
    public Type Type;
    public string MenuName;
    public FieldInfo DurationField;
}

public static class RobotCommandRegistry
{
    private static List<RobotCommandInfo> _infos;

    static RobotCommandRegistry()
    {
        Load();
    }

    private static void Load()
    {
        _infos = new List<RobotCommandInfo>();
        foreach (var type in TypeCache.GetTypesDerivedFrom<RobotCommand>())
        {
            if (type.IsAbstract)
                continue;
            var attr = type.GetCustomAttribute<RobotCommandAttribute>();
            var info = new RobotCommandInfo
            {
                Type = type,
                MenuName = attr?.MenuName ?? ObjectNames.NicifyVariableName(type.Name),
                DurationField = !string.IsNullOrEmpty(attr?.DurationField)
                    ? type.GetField(attr.DurationField, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    : null
            };
            _infos.Add(info);
        }
    }

    public static void PopulateMenu(GenericMenu menu, Action<Type> onSelected)
    {
        foreach (var info in _infos)
        {
            var t = info.Type;
            menu.AddItem(new GUIContent(info.MenuName), false, () => onSelected(t));
        }
    }

    public static void SetDuration(IRobotCommand command, float value)
    {
        var info = _infos.FirstOrDefault(i => i.Type == command.GetType());
        if (info != null && info.DurationField != null)
        {
            info.DurationField.SetValue(command, value);
        }
    }
}
