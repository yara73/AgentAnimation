using System.Collections;
using UnityEngine;

public interface IRobotCommand
{
    IEnumerator Execute(GameObject robot, Renderer renderer);
    float GetDuration();
    void ApplyState(ref RobotState state, float time);
}
