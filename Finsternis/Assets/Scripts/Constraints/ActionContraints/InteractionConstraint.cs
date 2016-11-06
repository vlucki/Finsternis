namespace Finsternis
{
    using UnityEngine;
    using System.Collections;

    public abstract class InteractionConstraint : Constraint
    {
        public abstract bool Check(EntityAction action);
    }

    public abstract class InteractionConstraint<T> : InteractionConstraint where T : EntityAction
    {

        public override bool Check(EntityAction action)
        {
            switch (type)
            {
                case ConstraintType.ALLOW:
                    return action is T;
                case ConstraintType.IGNORE:
                    return !(action is T);
            }

            return false;
        }
    }
}