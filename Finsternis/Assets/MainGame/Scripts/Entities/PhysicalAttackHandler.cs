using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class PhysicalAttackHandler : Trigger
{
    public Entity owner;
    public Vector2 impactMultiplier = Vector2.one;
    public ForceMode modeOnImpact = ForceMode.VelocityChange;
    public Vector2 executionMultiplier = Vector2.one;
    public ForceMode modeOnExecution = ForceMode.Impulse;
    public bool ActiveOnDeath = false;
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
        Ignore(owner.gameObject);
        onEnter.AddListener(DoCollide);
    }

    void DoCollide(GameObject collidedObject)
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
            owner.GetComponent<AttackAction>().Perform(otherChar, strBonus);
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
