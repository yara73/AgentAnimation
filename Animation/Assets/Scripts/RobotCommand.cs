using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class RobotCommand
{
    public abstract IEnumerator Execute(GameObject robot, Renderer renderer);
}

[System.Serializable]
public class MoveCommand : RobotCommand
{
    public Vector3 position;
    public float duration = 1f;

    public override IEnumerator Execute(GameObject robot, Renderer renderer)
    {
        Vector3 start = robot.transform.position;
        Vector3 target = position;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = duration > 0f ? time / duration : 1f;
            robot.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
        robot.transform.position = target;
    }
}

[System.Serializable]
public class RotateCommand : RobotCommand
{
    public Vector3 rotation;
    public float duration = 1f;

    public override IEnumerator Execute(GameObject robot, Renderer renderer)
    {
        Vector3 start = robot.transform.eulerAngles;
        Vector3 target = rotation;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = duration > 0f ? time / duration : 1f;
            robot.transform.eulerAngles = Vector3.Lerp(start, target, t);
            yield return null;
        }
        robot.transform.eulerAngles = target;
    }
}

[System.Serializable]
public class ColorCommand : RobotCommand
{
    public Color color = Color.white;
    public float duration = 1f;

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
}

[System.Serializable]
public class WaitCommand : RobotCommand
{
    public float time = 1f;

    public override IEnumerator Execute(GameObject robot, Renderer renderer)
    {
        yield return new WaitForSeconds(time);
    }
}
