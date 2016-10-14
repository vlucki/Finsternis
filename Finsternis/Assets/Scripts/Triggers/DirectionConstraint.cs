namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using UnityQuery;

    [CreateAssetMenu(fileName = "AngleTriggerConstraint", menuName = "Finsternis/Triggers/Constraints/Angle")]
    public class DirectionConstraint : TriggerConstraint
    {
        [SerializeField]
        [Range(0, 360)]
        private float maxAllowedAngle = 90;

        public override bool Check(Trigger t, Collider c)
        {
            return t.transform.position.Towards(c.transform.position).Angle(t.transform.forward) < maxAllowedAngle;
        }
    }
}