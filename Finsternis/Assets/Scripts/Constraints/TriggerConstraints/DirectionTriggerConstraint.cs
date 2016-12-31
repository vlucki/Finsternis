namespace Finsternis
{
    using UnityEngine;
    using Extensions;

    [CreateAssetMenu(fileName = "DirectionTriggerConstraint", menuName = "Finsternis/Triggers/Constraint/Direction")]
    public class FacingTriggerConstraint : TriggerConstraint
    {
        [SerializeField, Range(0, 180f)]
        private float angle = 90f;

        public override bool Check(Trigger t, Collider c)
        {
            var triggerTransform = t.transform;
            var colliderTransform = c.transform;

            var direction = triggerTransform.position - colliderTransform.position;
            var angle = colliderTransform.forward.Angle(direction);

            return angle <= this.angle && type == ConstraintType.ALLOW;
        }
    }
}