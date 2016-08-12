using System;
using UnityEngine;

public class TimeConstraint : IConstraint
{
    public float Duration { get; private set; }

    public float StartTime { get; private set; }

    public TimeConstraint(float duration)
    {
        this.Duration = duration;
        Reset();
    }

    public void Reset()
    {
        this.StartTime = Time.time;
    }

    public bool IsValid()
    {
        return Time.time - this.StartTime < Duration;
    }

    public bool AllowMultiple()
    {
        return false;
    }
}

