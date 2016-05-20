using UnityEngine;
using System.Collections;
using System;

public class Attack : EntityAction
{
    RangedValueAttribute damage;

    protected override void Awake()
    {
        base.Awake();
        if (!damage)
            damage = agent.GetAttribute("dmg") as RangedValueAttribute;
        if (!damage)
        {
            damage = gameObject.AddComponent<RangedValueAttribute>();
            damage.name = "dmg";
        }
    }

    public override void Perform(params object[] parameters)
    {
        int index = 0;
        Entity target = parameters[index] as Entity;
        index++;
        DamageInfo.DamageType dmgType = DamageInfo.DamageType.physical;
        if (parameters.Length > index && parameters[index] != null)
        {
            dmgType = (DamageInfo.DamageType)parameters[index];
            index++;
        }

        float amount = damage.Value;
        if (parameters.Length > index && parameters[index] != null)
        {
            amount += Convert.ToSingle(parameters[index]);
        }
        DamageInfo dmgInfo = new DamageInfo(dmgType, amount, agent);
        target.ReceiveDamage(dmgInfo);
    }
}
