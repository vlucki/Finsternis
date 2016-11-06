namespace Finsternis
{
    using UnityEngine;
    using System.Collections;

    public abstract class TriggerConstraint : Constraint
    {
        public abstract bool Check(Trigger t, Collider c);
    }
}