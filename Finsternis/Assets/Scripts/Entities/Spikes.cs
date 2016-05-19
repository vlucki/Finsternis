using UnityEngine;
using System.Collections;

public class Spikes : Trap
{
    public override void ReceiveDamage(DamageInfo info)
    {
        attack.Perform(info.Source, DamageInfo.DamageType.physical);
    }
}
