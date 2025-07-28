using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RobotExecutor : MonoBehaviour
{
    public RobotCommandSequence sequence;
    public RobotCommandTimeline timeline;
    public float timeScale = 1f;

    private Renderer _renderer;
    private Coroutine _routine;
    private RobotState _initialState;
    private bool _cachedState = false;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        CacheInitialState();
        RobotTime.TimeScale = timeScale;
        if (_routine != null) return;

        if (timeline)
            _routine = StartCoroutine(RunTimeline());
        else if (sequence)
            _routine = StartCoroutine(RunSequence());
    }

    public void Play()
    {
        if (_routine != null) return;

        if (_renderer == null)
            _renderer = GetComponent<Renderer>();

        if (!_cachedState)
            CacheInitialState();
        RobotTime.TimeScale = timeScale;
        if (timeline)
            _routine = StartCoroutine(RunTimeline());
        else if (sequence)
            _routine = StartCoroutine(RunSequence());
    }

    public void Stop()
    {
        if (_routine != null)
            StopAllCoroutines();
        _routine = null;
        RobotTime.TimeScale = 1f;
        if (_cachedState)
            ApplyState(_initialState);
    }

    public void Preview(float time)
    {
        if (!timeline)
            return;
        _renderer = GetComponent<Renderer>();

        var sorted = new List<RobotTimedCommand>(timeline.commands);
        sorted.Sort((a, b) => a.startTime.CompareTo(b.startTime));

        if (!_cachedState)
            CacheInitialState();

        var state = _initialState;

        foreach (var entry in sorted)
        {
            if (entry.command == null)
                continue;

            var local = time - entry.startTime;
            if (local <= 0f)
                continue;

            var dur = entry.command.GetDuration();
            if (local > dur)
                local = dur;

            entry.command.ApplyState(ref state, local);
        }

        ApplyState(state);
    }

    private IEnumerator RunSequence()
    {
        if (!sequence)
        {
            _routine = null;
            yield break;
        }

        do
        {
            if (_cachedState)
                ApplyState(_initialState);
            foreach (var command in sequence.commands.Where(command => command != null))
            {
                yield return StartCoroutine(command.Execute(gameObject, _renderer));
            }
        } while (sequence.loop);

        _routine = null;
    }

    private IEnumerator RunTimeline()
    {
        if (!timeline || timeline.commands.Count == 0)
        {
            _routine = null;
            yield break;
        }

        do
        {
            if (_cachedState)
                ApplyState(_initialState);
            var maxTime = 0f;
            foreach (var entry in timeline.commands.Where(entry => entry.command != null))
            {
                StartCoroutine(RunTimed(entry));
                var end = entry.startTime + entry.command.GetDuration();
                if (end > maxTime)
                    maxTime = end;
            }

            if (maxTime > 0f)
                yield return new WaitForSeconds(maxTime / RobotTime.TimeScale);
            else
                yield return null;
        } while (timeline.loop);

        _routine = null;
    }

    private IEnumerator RunTimed(RobotTimedCommand entry)
    {
        if (entry.startTime > 0f)
            yield return new WaitForSeconds(entry.startTime / RobotTime.TimeScale);
        yield return StartCoroutine(entry.command.Execute(gameObject, _renderer));
    }

    private void ApplyState(RobotState state)
    {
        transform.position = state.Position;
        transform.eulerAngles = state.Rotation;
        transform.localScale = state.Scale;
        if (_renderer)
            _renderer.sharedMaterial.color = state.Color;
        gameObject.SetActive(state.Active);
    }

    public void RefreshInitialState()
    {
        CacheInitialState();
    }

    private void CacheInitialState()
    {
        if (_renderer == null)
            _renderer = GetComponent<Renderer>();
        _initialState = new RobotState
        {
            Position = transform.position,
            Rotation = transform.eulerAngles,
            Scale = transform.localScale,
            Color = _renderer ? _renderer.sharedMaterial.color : Color.white,
            Active = gameObject.activeSelf
        };
        _cachedState = true;
    }
}
