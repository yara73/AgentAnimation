using UnityEngine;
using System.Collections;

[System.Serializable]
[RobotCommand("Scale", "duration")]
public class ScaleCommand : RobotCommand
{
    public Vector3 scale = Vector3.one;
    public float duration = 1f;

    public override float GetDuration() => duration;

    public override IEnumerator Execute(GameObject robot, Renderer renderer)
    {
        Vector3 start = robot.transform.localScale;
        Vector3 target = scale;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime * RobotTime.TimeScale;
            float t = duration > 0f ? time / duration : 1f;
            robot.transform.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }
        robot.transform.localScale = target;
    }

    public override void ApplyState(ref RobotState state, float time)
    {
        float t = duration > 0f ? Mathf.Clamp01(time / duration) : 1f;
        state.Scale = Vector3.Lerp(state.Scale, scale, t);
    }
}
