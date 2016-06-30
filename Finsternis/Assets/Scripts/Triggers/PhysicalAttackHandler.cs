using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Collider))]
public class PhysicalAttackHandler : Trigger
{
    public Entity owner;
    public Vector2 impactMultiplier = Vector2.one;
    public ForceMode modeOnImpact = ForceMode.VelocityChange;
    public Vector2 executionMultiplier = Vector2.one;
    public ForceMode modeOnExecution = ForceMode.Impulse;
    public bool activeOnDeath = false;
    public bool ignoreOthersFromOwner = true;


    private RangedValueAttribute str;


    void Awake()
    {
        if (!owner)
        {
            owner = GetComponentInParent<Entity>();
            if (!owner)
                throw new System.InvalidOperationException("Attack handler needs an owner!");

        }
        str = owner.GetAttribute("str") as RangedValueAttribute;
        if (!activeOnDeath)
        {
            RangedValueAttribute health = owner.GetAttribute("hp") as RangedValueAttribute;
            if (health)
                health.onValueChanged.AddListener(HealthChanged);
        }
        Ignore(owner.gameObject);
        onEnter.AddListener(DoCollide);
    }

    protected override bool ShouldTrigger(Collider other)
    {
        bool result = base.ShouldTrigger(other);
        if(result && ignoreOthersFromOwner)
        {
            PhysicalAttackHandler pah = other.GetComponent<PhysicalAttackHandler>();
            if (pah && pah.owner == owner)
                result = false;
        }

        return result;
    }

    private void HealthChanged(EntityAttribute attribute)
    {
        if(attribute.Value <= 0)
        {
            GetComponent<Collider>().enabled = activeOnDeath;
        }
    }

    private void DoCollide(GameObject collidedObject)
    {
        Collider other = collidedObject.GetComponent<Collider>();
        if (ignoreColliders != null && ignoreColliders.Contains(other))
            return;

        Entity otherChar = collidedObject.GetComponentInParent<Entity>();

        if (otherChar)
        {
            if (otherChar is Character && ((Character)otherChar).Invincible)
                return;

            CharacterController controller = collidedObject.GetComponent<CharacterController>();
            if (controller)
            {
                controller.Hit();
            }
            float strBonus = str ? str.Value * 0.5f : 0;
            AttackAction attack = owner.GetComponent<AttackAction>();

            if(attack)
                attack.Perform(otherChar, strBonus);
        }
        SimulateImpact(collidedObject, impactMultiplier, true);
    }

    private void SimulateImpact(GameObject other, Vector2 multiplier, bool zeroVelocity = false)
    {
        Rigidbody body = other.GetComponentInParent<Rigidbody>();
        if (body)
        {
            if (zeroVelocity)
                body.velocity = Vector3.zero;

            Vector3 dir = other.transform.position - transform.position;
            dir.y = 0;
            dir.Normalize();

            float strModifier = str ? str.Value : 1;
            body.AddForce((dir * multiplier.x + Vector3.up * multiplier.y) * strModifier, modeOnImpact);
        }
    }
}
