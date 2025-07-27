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

    public void Preview(float time)
    {
        if (timeline == null)
            return;
        _renderer = GetComponent<Renderer>();

        var sorted = new System.Collections.Generic.List<RobotTimedCommand>(timeline.commands);
        sorted.Sort((a, b) => a.startTime.CompareTo(b.startTime));

        RobotState state = new RobotState
        {
            Position = transform.position,
            Rotation = transform.eulerAngles,
            Color = _renderer ? _renderer.sharedMaterial.color : Color.white
        };

        foreach (var entry in sorted)
        {
            if (entry.command == null)
                continue;
            float start = entry.startTime;
            float dur = entry.command.GetDuration();
            if (time >= start + dur)
            {
                entry.command.ApplyState(ref state, dur);
            }
            else if (time >= start)
            {
                entry.command.ApplyState(ref state, time - start);
                break;
            }
        }

        ApplyState(state);
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

    void ApplyState(RobotState state)
    {
        transform.position = state.Position;
        transform.eulerAngles = state.Rotation;
        if (_renderer != null)
            _renderer.sharedMaterial.color = state.Color;
    }
}
