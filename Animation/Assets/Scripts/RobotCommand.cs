using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class RobotCommand : IRobotCommand
{
    public abstract IEnumerator Execute(GameObject robot, Renderer renderer);

    public virtual float GetDuration() => 0f;

    public abstract void ApplyState(ref RobotState state, float time);
}
