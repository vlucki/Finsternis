using UnityEngine;

public class TimeConstraint : IConstraint
{
    private float startTime;

    public float Duration { get; private set; }

    public TimeConstraint(float duration)
    {
        this.Duration = duration;
        Reset();
    }

    public void Reset()
    {
        this.startTime = Time.time;
    }

    public bool IsValid()
    {
        return Time.time - startTime < Duration;
    }

    public bool AllowMultiple()
    {
        return false;
    }
}

