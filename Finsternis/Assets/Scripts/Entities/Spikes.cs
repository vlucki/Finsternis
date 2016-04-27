using UnityEngine;
using System.Collections;

public class Spikes : Trap
{
    public override void ReceiveDamage(DamageInfo info)
    {
        DoDamage(info.Source);
    }
}
