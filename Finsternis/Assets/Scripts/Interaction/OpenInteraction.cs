namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    [RequireComponent(typeof(OpeneableEntity))]
    public class OpenInteraction : Interaction
    {
        protected override bool Interact(InteractionData data)
        {
            if (!base.ValidadeInteractionType(data))
                return false;

            OpenData oData = data as OpenData;
            if (!oData)
                return false;

            OpeneableEntity thisEntity = GetComponent<OpeneableEntity>();
            var keys = oData.Keys;
            if (thisEntity.IsLocked)
            {
                
            }

            return true;
        }
    }
}