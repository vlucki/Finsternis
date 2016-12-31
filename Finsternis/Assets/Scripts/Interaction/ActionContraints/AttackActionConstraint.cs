
namespace Finsternis
{
    using System;
    using UnityEngine;
    [CreateAssetMenu(fileName = "AttackActionConstraint", menuName = "Finsternis/Action Constraints/Attack Action")]
    public class AttackActionConstraint : InteractionConstraint<AttackData>
    {
        public override bool Check(InteractionData data)
        {
            throw new NotImplementedException();
        }
    }
}
