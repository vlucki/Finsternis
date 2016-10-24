namespace Finsternis
{
    using UnityEngine;
    using System.Collections;

    public abstract class TriggerConstraint : ScriptableObject
    {
        public abstract bool Check(Trigger t, Collider c);
    }
}