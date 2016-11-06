namespace Finsternis
{
    using System;
    using UnityEngine;

    public class TimeConstraint : EffectConstraint
    {
        public float Duration { get; private set; }

        public float StartTime { get; private set; }

        public void Reset()
        {
            this.StartTime = Time.time;
        }

        public bool AllowMultiple()
        {
            return false;
        }

        public override bool IsValid(Effect e)
        {
            return Time.time - this.StartTime < Duration;
        }
    }
}