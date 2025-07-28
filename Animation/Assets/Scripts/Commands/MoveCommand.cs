using UnityEngine;
using System.Collections;

[System.Serializable]
[RobotCommand("Move", "duration")]
public class MoveCommand : RobotCommand
{
    public Vector3 position;
    public float duration = 1f;

    public override float GetDuration() => duration;

    public override IEnumerator Execute(GameObject robot, Renderer renderer)
    {
        var start = robot.transform.position;
        var target = position;
        var time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime * RobotTime.TimeScale;
            float t = duration > 0f ? time / duration : 1f;
            robot.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
        robot.transform.position = target;
    }

    public override void ApplyState(ref RobotState state, float time)
    {
        var t = duration > 0f ? Mathf.Clamp01(time / duration) : 1f;
        state.Position = Vector3.Lerp(state.Position, position, t);
    }
}
