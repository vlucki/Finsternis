using UnityEngine;
using System.Collections.Generic;
using System;
namespace Finsternis
{
    public class PhysicalAttackHandler : Trigger
    {
        public Entity owner;
        public Vector2 impactMultiplier = Vector2.one;
        public ForceMode modeOnImpact = ForceMode.VelocityChange;
        public Vector2 executionMultiplier = Vector2.one;
        public ForceMode modeOnExecution = ForceMode.Impulse;
        public bool activeOnDeath = false;
        public bool ignoreOthersFromOwner = true;

        private EntityAttribute str;

        protected override void Awake()
        {
            base.Awake();
            if (!owner)
            {
                owner = GetComponentInParent<Entity>();
                if (!owner)
                    throw new System.InvalidOperationException("Attack handler needs an owner!");

            }
            this.str = owner.GetAttribute("str") as EntityAttribute;
            if (!activeOnDeath)
            {
                EntityAttribute health = owner.GetAttribute("hp") as EntityAttribute;
                if (health)
                    health.onValueChanged.AddListener(HealthChanged);
            }
            Ignore(owner.gameObject);
            onEnter.AddListener(DoCollide);
        }

        protected override bool ShouldTrigger(Collider other)
        {
            bool result = base.ShouldTrigger(other);
            if (result && ignoreOthersFromOwner)
            {
                PhysicalAttackHandler pAtkHandler = other.GetComponent<PhysicalAttackHandler>();
                if (pAtkHandler && pAtkHandler.owner == owner)
                    result = false;
            }

            return result;
        }

        private void HealthChanged(EntityAttribute attribute)
        {
            if (attribute.Value <= 0)
            {
                collider.enabled = activeOnDeath;
            }
        }

        private void DoCollide(GameObject collidedObject)
        {
            Collider other = collidedObject.GetComponent<Collider>();
            if (ignoreColliders != null && ignoreColliders.Contains(other))
                return;

            IInteractable interactable = collidedObject.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                if (interactable is Entity)
                {
                    CharController controller = collidedObject.GetComponent<CharController>();
                    if (controller) controller.Hit();
                }

                float strBonus = str ? str.Value * 0.5f : 0;
                AttackAction attack = owner.GetComponent<AttackAction>();

                if (attack)
                    attack.Execute(strBonus, interactable);
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
}