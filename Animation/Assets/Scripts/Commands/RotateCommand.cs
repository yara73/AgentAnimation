using UnityEngine;
using System.Collections;

[System.Serializable]
[RobotCommand("Rotate", "duration")]
public class RotateCommand : RobotCommand
{
    public Vector3 rotation;
    public float duration = 1f;

    public override float GetDuration() => duration;

    public override IEnumerator Execute(GameObject robot, Renderer renderer)
    {
        var start = robot.transform.eulerAngles;
        var target = rotation;
        var time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            var t = duration > 0f ? time / duration : 1f;
            robot.transform.eulerAngles = Vector3.Lerp(start, target, t);
            yield return null;
        }
        robot.transform.eulerAngles = target;
    }

    public override void ApplyState(ref RobotState state, float time)
    {
        var t = duration > 0f ? Mathf.Clamp01(time / duration) : 1f;
        state.Rotation = Vector3.Lerp(state.Rotation, rotation, t);
    }
}
