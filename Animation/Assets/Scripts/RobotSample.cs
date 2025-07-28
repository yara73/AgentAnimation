using UnityEngine;

public class RobotSample : MonoBehaviour
{
    public RobotCommandSequence sequence;

    private void Start()
    {
        var executor = gameObject.GetOrAddComponent<RobotExecutor>();
        executor.sequence = sequence;
        executor.Play();
    }
}
