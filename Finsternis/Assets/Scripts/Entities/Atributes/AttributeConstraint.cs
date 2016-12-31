namespace Finsternis
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class AttributeConstraint : ValueConstraint
    {
        public enum AttributeConstraintType
        { MIN = -1, MAX = 1 }

        public AttributeConstraintType Type { get; set; }

        public override bool AllowMultiple()
        {
            return true;
        }

        public override bool Validate(float value)
        {
            if (value < this.Value && this.Type == AttributeConstraintType.MIN)
            {
                return false;
            }

            if (value > this.Value && this.Type == AttributeConstraintType.MAX)
            {
                return false;
            }

            return true;
        }

        public override int CompareTo(ValueConstraint other)
        {
            int result = 1;

            if (other)
                result = this.Value.CompareTo(other.Value);

            return result;
        }

        public override bool Equals(ValueConstraint other)
        {
            if (!base.Equals(other))
                return false;

            var otherAttributeConstraint = other as AttributeConstraint;
            if (!otherAttributeConstraint)
                return false;

            return otherAttributeConstraint.Type == this.Type;
        }
    }
}