using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Robot/Command Sequence", fileName = "RobotSequence")]
public class RobotCommandSequence : ScriptableObject
{
    [SerializeReference]
    public List<RobotCommand> commands = new List<RobotCommand>();
}
