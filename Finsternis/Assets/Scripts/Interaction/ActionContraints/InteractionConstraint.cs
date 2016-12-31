namespace Finsternis
{
    using UnityEngine;
    using System.Collections;

    public abstract class InteractionConstraint : Constraint
    {
        public abstract bool Check(InteractionData data);
    }

    public abstract class InteractionConstraint<T> : InteractionConstraint where T : InteractionData
    {
        public override bool Check(InteractionData data)
        {
            return !(data is T) || (type == ConstraintType.ALLOW);
        }
    }
}