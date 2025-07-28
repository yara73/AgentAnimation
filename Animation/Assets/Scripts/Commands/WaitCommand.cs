using UnityEngine;
using System.Collections;

[System.Serializable]
[RobotCommand("Wait", "time")]
public class WaitCommand : RobotCommand
{
    public float time = 1f;

    public override float GetDuration() => time;

    public override IEnumerator Execute(GameObject robot, Renderer renderer)
    {
        yield return new WaitForSeconds(time / RobotTime.TimeScale);
    }

    public override void ApplyState(ref RobotState state, float time)
    {
    }
}
