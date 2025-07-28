using UnityEngine;
using System.Collections;

[System.Serializable]
[RobotCommand("Fade", "duration")]
public class FadeCommand : RobotCommand
{
    public float alpha = 1f;
    public float duration = 1f;

    public override float GetDuration() => duration;

    public override IEnumerator Execute(GameObject robot, Renderer renderer)
    {
        if (renderer == null)
            yield break;
        Color start = renderer.sharedMaterial.color;
        Color target = new Color(start.r, start.g, start.b, alpha);
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime * RobotTime.TimeScale;
            float t = duration > 0f ? time / duration : 1f;
            renderer.sharedMaterial.color = Color.Lerp(start, target, t);
            yield return null;
        }
        renderer.sharedMaterial.color = target;
    }

    public override void ApplyState(ref RobotState state, float time)
    {
        float t = duration > 0f ? Mathf.Clamp01(time / duration) : 1f;
        var start = state.Color;
        var target = new Color(start.r, start.g, start.b, alpha);
        state.Color = Color.Lerp(start, target, t);
    }
}
