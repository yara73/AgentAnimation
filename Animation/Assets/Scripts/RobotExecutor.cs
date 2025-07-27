using UnityEngine;
using System.Collections;

public class RobotExecutor : MonoBehaviour
{
    public RobotCommandSequence sequence;
    public RobotCommandTimeline timeline;

    private Renderer _renderer;
    private Coroutine _routine;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (timeline != null)
            _routine = StartCoroutine(RunTimeline());
        else if (sequence != null)
            _routine = StartCoroutine(RunSequence());
    }

    public void Play()
    {
        if (_routine == null)
        {
            if (timeline != null)
                _routine = StartCoroutine(RunTimeline());
            else if (sequence != null)
                _routine = StartCoroutine(RunSequence());
        }
    }

    public void Stop()
    {
        if (_routine != null)
            StopCoroutine(_routine);
        _routine = null;
    }

    IEnumerator RunSequence()
    {
        if (sequence == null)
        {
            _routine = null;
            yield break;
        }

        do
        {
            foreach (var command in sequence.commands)
            {
                if (command != null)
                    yield return StartCoroutine(command.Execute(gameObject, _renderer));
            }
        } while (sequence.loop);

        _routine = null;
    }

    IEnumerator RunTimeline()
    {
        if (timeline == null || timeline.commands.Count == 0)
        {
            _routine = null;
            yield break;
        }

        do
        {
            var routines = new System.Collections.Generic.List<Coroutine>();
            foreach (var entry in timeline.commands)
            {
                if (entry.command == null)
                    continue;
                routines.Add(StartCoroutine(RunTimed(entry)));
            }

            foreach (var r in routines)
                yield return r;
        } while (timeline.loop);

        _routine = null;
    }

    IEnumerator RunTimed(RobotTimedCommand entry)
    {
        if (entry.startTime > 0f)
            yield return new WaitForSeconds(entry.startTime);
        yield return StartCoroutine(entry.command.Execute(gameObject, _renderer));
    }
}
