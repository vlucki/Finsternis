namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "TagTriggerConstraint", menuName = "Finsternis/Triggers/Constraints/Tag")]
    public class TagTriggerConstraint : TriggerConstraint
    {
        [SerializeField][TagSelection]
        private List<string> allowedTags = new List<string>();

        public override bool Check(Trigger t, Collider c)
        {
            return allowedTags.Contains(c.tag);
        }
    }
}