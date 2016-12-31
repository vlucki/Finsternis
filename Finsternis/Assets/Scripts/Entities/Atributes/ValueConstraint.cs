namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using System.Collections.Generic;

    public abstract class ValueConstraint : IConstraint<float>, IComparable<ValueConstraint>, IEquatable<ValueConstraint>
    {
        public float Value { get; set; }

        public static implicit operator bool(ValueConstraint constraint) { return constraint != null; }

        public abstract bool AllowMultiple();

        public abstract bool Validate(float value);

        public abstract int CompareTo(ValueConstraint other);

        public virtual bool Equals(ValueConstraint other)
        {
            if (!other)
                return false;

            return other.Value == this.Value;
        }
    }
}