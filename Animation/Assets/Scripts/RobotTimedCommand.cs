using System;
using UnityEngine;

[Serializable]
public class RobotTimedCommand
{
    public float startTime;
    [SerializeReference]
    public RobotCommand command;
}
