using UnityEngine;
using System.Collections;

public class Spikes : Trap
{
    public override void Interact(EntityAction action)
    {
        attack.Perform(action.Agent, DamageInfo.DamageType.physical);
    }
}
