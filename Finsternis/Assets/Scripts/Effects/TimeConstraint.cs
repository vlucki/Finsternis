using UnityEngine;

public class TimeConstraint : IConstraint
{
    private float startTime;

    public float Duration { get; private set; }

    public TimeConstraint(float duration)
    {
        this.startTime = Time.time;
        this.Duration = duration;
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

