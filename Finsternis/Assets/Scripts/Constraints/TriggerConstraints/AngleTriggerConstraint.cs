namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using UnityQuery;

    [CreateAssetMenu(fileName = "AngleTriggerConstraint", menuName = "Finsternis/Triggers/Constraints/Angle")]
    public class AngleTriggerConstraint : TriggerConstraint
    {
        [SerializeField]
        [Range(0, 360)]
        private float maxAllowedAngle = 90;

        public override bool Check(Trigger t, Collider c)
        {
            var dir = t.transform.position.Towards(c.transform.position);
            var angle = dir.Angle(t.transform.forward);
            if (angle > 180)
                angle -= 180;
            
            return angle <= maxAllowedAngle;
        }
    }
}