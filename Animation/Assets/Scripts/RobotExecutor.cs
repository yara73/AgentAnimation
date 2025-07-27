using UnityEngine;
using System.Collections;

public class RobotExecutor : MonoBehaviour
{
    public RobotCommandSequence sequence;
    public RobotCommandTimeline timeline;

    private Renderer _renderer;
    private Coroutine _routine;

    struct State
    {
        public Vector3 position;
        public Vector3 rotation;
        public Color color;
    }

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

        State state;
        state.position = transform.position;
        state.rotation = transform.eulerAngles;
        state.color = _renderer ? _renderer.sharedMaterial.color : Color.white;

        foreach (var entry in sorted)
        {
            if (entry.command == null)
                continue;
            float start = entry.startTime;
            float dur = entry.command.GetDuration();
            if (time >= start + dur)
            {
                ApplyEnd(entry.command, ref state);
            }
            else if (time >= start)
            {
                ApplyProgress(entry.command, ref state, time - start, dur);
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

    void ApplyState(State state)
    {
        transform.position = state.position;
        transform.eulerAngles = state.rotation;
        if (_renderer != null)
            _renderer.sharedMaterial.color = state.color;
    }

    void ApplyEnd(RobotCommand command, ref State state)
    {
        if (command is MoveCommand m)
            state.position = m.position;
        else if (command is RotateCommand r)
            state.rotation = r.rotation;
        else if (command is ColorCommand c)
            state.color = c.color;
    }

    void ApplyProgress(RobotCommand command, ref State state, float t, float duration)
    {
        float pct = duration > 0f ? t / duration : 1f;
        if (command is MoveCommand m)
            state.position = Vector3.Lerp(state.position, m.position, pct);
        else if (command is RotateCommand r)
            state.rotation = Vector3.Lerp(state.rotation, r.rotation, pct);
        else if (command is ColorCommand c)
            state.color = Color.Lerp(state.color, c.color, pct);
    }
}
