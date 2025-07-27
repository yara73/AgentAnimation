using UnityEngine;

public class RobotSample : MonoBehaviour
{
    public RobotCommandSequence sequence;

    void Start()
    {
        var executor = GetComponent<RobotExecutor>();
        if (executor == null)
            executor = gameObject.AddComponent<RobotExecutor>();
        executor.sequence = sequence;
        executor.Play();
    }
}
