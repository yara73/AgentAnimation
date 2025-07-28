using UnityEngine;
using System.Collections;

[System.Serializable]
[RobotCommand("Jump", "duration")]
public class JumpCommand : RobotCommand
{
    public float height = 1f;
    public float duration = 1f;

    public override float GetDuration() => duration;

    public override IEnumerator Execute(GameObject robot, Renderer renderer)
    {
        Vector3 start = robot.transform.position;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime * RobotTime.TimeScale;
            float t = duration > 0f ? time / duration : 1f;
            float offset = Mathf.Sin(t * Mathf.PI) * height;
            robot.transform.position = start + Vector3.up * offset;
            yield return null;
        }
        robot.transform.position = start;
    }

    public override void ApplyState(ref RobotState state, float time)
    {
        float t = duration > 0f ? Mathf.Clamp01(time / duration) : 1f;
        float offset = Mathf.Sin(t * Mathf.PI) * height;
        state.Position = state.Position + Vector3.up * offset;
    }
}
