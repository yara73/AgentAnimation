using UnityEngine;
using System.Collections;

public class RobotExecutor : MonoBehaviour
{
    public RobotCommandSequence sequence;

    private Renderer _renderer;
    private Coroutine _routine;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (sequence != null)
            _routine = StartCoroutine(Run());
    }

    public void Play()
    {
        if (sequence != null && _routine == null)
            _routine = StartCoroutine(Run());
    }

    public void Stop()
    {
        if (_routine != null)
            StopCoroutine(_routine);
        _routine = null;
    }

    IEnumerator Run()
    {
        foreach (var command in sequence.commands)
        {
            if (command != null)
                yield return StartCoroutine(command.Execute(gameObject, _renderer));
        }
        _routine = null;
    }
}
