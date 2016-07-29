using UnityEngine;

public class TimeConstraint : IConstraint
{
    private float startTime;
    private float duration;

    public TimeConstraint(float duration)
    {
        this.startTime = Time.time;
        this.duration = duration;
    }

    public bool IsValid()
    {
        return Time.time - startTime < duration;
    }

    public bool AllowMultiple()
    {
        return false;
    }
}

