using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class PhysicalAttackHandler : MonoBehaviour
{
    public Entity owner;
    public List<Collider> ignoreList;
    public Vector2 impactMultiplier = new Vector2(2, 3);
    public ForceMode modeOnImpact = ForceMode.VelocityChange;
    public Vector2 executionMultiplier = new Vector2(10, 2);
    public ForceMode modeOnExecution = ForceMode.Impulse;

    void Awake()
    {
        if (!owner)
        {
            owner = GetComponentInParent<Entity>();
            if (!owner)
                throw new System.InvalidOperationException("Attack handler needs an owner!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (ignoreList != null)
            if (ignoreList.Contains(other))
                return;

        if (!owner.GetComponent<CharacterController>().IsAttacking())
            return;

        Rigidbody body = other.GetComponent<Rigidbody>();
        if (body)
        {
            Vector3 dir = (other.transform.position - owner.transform.position);
            body.AddForce(dir * impactMultiplier.x + Vector3.up * impactMultiplier.y, modeOnImpact);
            Vector3 target = owner.transform.position;
            target.y = other.transform.position.y;
            other.transform.LookAt(target);
        }

        Entity otherChar = other.GetComponent<Entity>();
        if (!otherChar)
            otherChar = other.GetComponentInParent<Entity>();

        if (otherChar)
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if(controller) controller.Hit();
            owner.DoDamage(otherChar);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!owner.GetComponent<CharacterController>().IsAttacking())
            return;

        if (ignoreList != null)
            if (ignoreList.Contains(other))
                return;

        Rigidbody body = other.GetComponent<Rigidbody>();
        if (body)
        {
            body.AddForce((other.transform.position - owner.transform.position) * executionMultiplier.x + Vector3.up * executionMultiplier.y, modeOnExecution);
        }
    }
}
