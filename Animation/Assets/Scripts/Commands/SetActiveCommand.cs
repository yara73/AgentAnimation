using UnityEngine;
using System.Collections;

[System.Serializable]
[RobotCommand("Set Active")]
public class SetActiveCommand : RobotCommand
{
    public bool active = true;

    public override float GetDuration() => 0f;

    public override IEnumerator Execute(GameObject robot, Renderer renderer)
    {
        robot.SetActive(active);
        yield break;
    }

    public override void ApplyState(ref RobotState state, float time)
    {
        state.Active = active;
    }
}
