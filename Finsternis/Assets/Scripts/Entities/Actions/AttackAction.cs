using System;
using System.Collections.Generic;

public class AttackAction : EntityAction
{
    EntityAttribute damage;
    private DamageInfo dmgInfo;

    public DamageInfo DamageInfo
    {
        get { return dmgInfo; }
    }

    protected override void Awake()
    {
        base.Awake();
        if (!damage)
            damage = agent.GetAttribute("dmg") as EntityAttribute;
        if (!damage)
        {
            damage = gameObject.AddComponent<EntityAttribute>();
            damage.name = "dmg";
        }
    }

    /// <summary>
    /// Performs an attack action.
    /// </summary>
    /// <param name="parameters">
    /// Every parameter needed to perform this action.</br>
    /// - (REQUIRED) One or more targeted Entities;
    /// - (OPTIONAL) Type of damage;
    /// - (OPTIONAL) Extra damage (from buffs and whatnot);
    /// </param>
    public override void Perform(params object[] parameters)
    {
        if (parameters.Length < 1)
            throw new ArgumentException("Cannot execute the attack logic without any parameters.");

        Entity[] targets;
        if (!GetParameters(parameters, out targets))
            throw new ArgumentException("Cannot execute the attack logic without a target.");

        DamageInfo.DamageType damageType;
        GetParameter(parameters, out damageType);

        float extraDamage;
        GetParameter(parameters, out extraDamage);

        float totalDamage = damage.Value + extraDamage;

        dmgInfo = new DamageInfo(damageType, totalDamage, agent);
        foreach (Entity target in targets)
            target.Interact(this);
    }
}
