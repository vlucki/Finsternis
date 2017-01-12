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
            if (thisEntity.IsLocked)
            {
                //go backwards since elements may be removed during iteration
                for(int keyIndex = oData.Keys.Count; keyIndex >= 0; keyIndex--)
                {
                    KeyCard key = oData.Keys[keyIndex];
                    for(int lockIndex = thisEntity.Locks.Count - 1; lockIndex >= 0; lockIndex--)
                    {
                        Lock l = thisEntity.Locks[lockIndex];
                        if (l.Unlock(key) && key.UsedUp)
                            break;
                    };
                }
            }

            return true;
        }
    }
}