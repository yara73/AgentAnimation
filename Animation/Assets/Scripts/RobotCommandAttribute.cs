using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class RobotCommandAttribute : Attribute
{
    public string MenuName { get; }
    public string DurationField { get; }

    public RobotCommandAttribute(string menuName, string durationField = null)
    {
        MenuName = menuName;
        DurationField = durationField;
    }
}
