using UnityEngine;

public class RobotSample : MonoBehaviour
{
    public RobotCommandSequence sequence;

    private void Start()
    {
        var executor = GetComponent<RobotExecutor>();
        if (!executor)
            executor = gameObject.AddComponent<RobotExecutor>();
        executor.sequence = sequence;
        executor.Play();
    }
}
