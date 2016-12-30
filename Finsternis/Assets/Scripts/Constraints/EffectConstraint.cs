namespace Finsternis
{
    using UnityEngine;
    using System.Collections;

    public abstract class EffectConstraint : Constraint
    {
        public abstract bool IsValid(Effect e);
    }
}