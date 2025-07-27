using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Robot/Command Timeline", fileName = "RobotTimeline")]
public class RobotCommandTimeline : ScriptableObject
{
    public List<RobotTimedCommand> commands = new List<RobotTimedCommand>();
}
