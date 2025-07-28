using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Robot/Command Sequence", fileName = "RobotSequence")]
public class RobotCommandSequence : ScriptableObject
{
    [SerializeReference]
    public List<IRobotCommand> commands = new List<IRobotCommand>();
    public bool loop = false;
}
