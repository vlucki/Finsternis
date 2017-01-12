namespace Finsternis
{
    using UnityEngine;

    public abstract class Constraint : ScriptableObject
    {

        public enum ConstraintType
        {
            ALLOW = 0,
            IGNORE = 1
        }

        [SerializeField]
        protected ConstraintType type = ConstraintType.ALLOW;
    }
}