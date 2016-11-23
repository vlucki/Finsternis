namespace Finsternis
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "TagTriggerConstraint", menuName = "Finsternis/Triggers/Constraint/Tag")]
    public class TagTriggerConstraint : TriggerConstraint
    {
        [SerializeField, TagSelection]
        private List<string> tags = new List<string>();
        
        public override bool Check(Trigger t, Collider c)
        {
            return tags.Contains(c.tag) == (type == ConstraintType.ALLOW);
        }

    }
}
