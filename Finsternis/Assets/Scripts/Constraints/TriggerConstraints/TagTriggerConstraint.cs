namespace Finsternis
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [CreateAssetMenu(fileName = "TagTriggerConstraint", menuName = "Finsternis/Tag")]
    public class TagTriggerConstraint : TriggerConstraint
    {
        [Serializable]
        public enum Mode
        {
            WHITELIST = 0,
            BLACKLIST = 1
        }

        [SerializeField]

        private Mode mode = Mode.WHITELIST;
        [SerializeField]
        [TagSelection]
        private List<string> tags = new List<string>();
        
        public override bool Check(Trigger t, Collider c)
        {
            if(this.mode == Mode.WHITELIST)
                return tags.Contains(c.tag);
            else
                return !tags.Contains(c.tag);
        }

    }
}
