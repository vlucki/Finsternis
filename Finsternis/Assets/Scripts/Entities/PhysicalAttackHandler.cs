using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class PhysicalAttackHandler : MonoBehaviour
{
    public Entity owner;
    public List<Collider> ignoreList;
    public Vector2 impactMultiplier = Vector2.one;
    public ForceMode modeOnImpact = ForceMode.VelocityChange;
    public Vector2 executionMultiplier = Vector2.one;
    public ForceMode modeOnExecution = ForceMode.Impulse;
    public bool ActiveOnDeath = false;


    void Awake()
    {
        if (!owner)
        {
            owner = GetComponentInParent<Entity>();
            if (!owner)
                throw new System.InvalidOperationException("Attack handler needs an owner!");
        }
    }

    void Update()
    {
        if (owner.Dead && !ActiveOnDeath)
            this.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        SimulateImpact(other, impactMultiplier);

        Entity otherChar = other.GetComponentInParent<Entity>();

        if (otherChar)
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if(controller) controller.Hit();
            owner.GetComponent<AttackAction>().Perform(otherChar);
        }
    }

    private void SimulateImpact(Collider other, Vector2 multiplier)
    {
        if (ignoreList != null && ignoreList.Contains(other))
            return;

        if (!owner.GetComponent<CharacterController>().IsAttacking())
            return;

        Rigidbody body = other.GetComponentInParent<Rigidbody>();
        if (body)
        {
            Vector3 dir = (other.transform.position - owner.transform.position);

            RangedValueAttribute str = owner.GetAttribute("str");

            float strModifier = str ? str.Value : 1; 

            body.AddForce((dir * multiplier.x + Vector3.up * multiplier.y) * strModifier, modeOnImpact);
            Vector3 target = owner.transform.position;
            target.y = other.transform.position.y;
            other.transform.LookAt(target);
        }
    }

    void OnTriggerStay(Collider other)
    {
        SimulateImpact(other, executionMultiplier);
    }
}
