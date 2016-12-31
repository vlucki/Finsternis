
namespace Finsternis
{
    using System;
    using UnityEngine;
    [CreateAssetMenu(fileName = "OpenActionConstraint", menuName = "Finsternis/Action Constraints/Open Action")]
    public class OpenActionConstraint : InteractionConstraint<OpenData>
    {
        public override bool Check(InteractionData data)
        {
            throw new NotImplementedException();
        }
    }
}
