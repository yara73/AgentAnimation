using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class RobotCommand : IRobotCommand
{
    public abstract IEnumerator Execute(GameObject robot, Renderer renderer);
    
    public virtual float GetDuration() => 0f;

    public abstract void ApplyState(ref RobotState state, float time);
}

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
            time += Time.deltaTime;
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

[System.Serializable]
[RobotCommand("Change Color", "duration")]
public class ColorCommand : RobotCommand
{
    public Color color = Color.white;
    public float duration = 1f;

    public override float GetDuration() => duration;

    public override IEnumerator Execute(GameObject robot, Renderer renderer)
    {
        if (renderer == null)
            yield break;

        Color start = renderer.material.color;
        Color target = color;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = duration > 0f ? time / duration : 1f;
            renderer.material.color = Color.Lerp(start, target, t);
            yield return null;
        }
        renderer.material.color = target;
    }

    public override void ApplyState(ref RobotState state, float time)
    {
        float t = duration > 0f ? Mathf.Clamp01(time / duration) : 1f;
        state.Color = Color.Lerp(state.Color, color, t);
    }
}

[System.Serializable]
[RobotCommand("Wait", "time")]
public class WaitCommand : RobotCommand
{
    public float time = 1f;

    public override float GetDuration() => time;

    public override IEnumerator Execute(GameObject robot, Renderer renderer)
    {
        yield return new WaitForSeconds(time);
    }

    public override void ApplyState(ref RobotState state, float time)
    {
        
    }
}
