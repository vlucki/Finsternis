using UnityEngine;
using System.Collections.Generic;
using System;
using UnityQuery;

namespace Finsternis
{
    public class TouchDamageHandler : Trigger
    {
        [SerializeField]
        private Entity owner;
        [SerializeField]
        private Vector2 impactMultiplier = Vector2.one;
        [SerializeField]
        private ForceMode modeOnImpact = ForceMode.VelocityChange;
        [SerializeField]
        private bool ignoreOthersFromOwner = true;

        [SerializeField]
        private List<EntityAttributeInfluence> attributesThatInfluenceDamage;

        [SerializeField]
        private List<EntityAttributeInfluence> attributesThatInfluenceImpact;

        public Entity Owner {
            get { return this.owner; }
            set { this.owner = value; }
        }

        void Start()
        {
            ValidateOwner();
            onEnter.AddListener(DoCollide);
        }

        void ValidateOwner()
        {
            if (!owner)
            {
                owner = GetComponentInParent<Entity>();
                if (!owner)
                    throw new System.InvalidOperationException("Attack handler needs an owner!");
            }
            owner.onAttributeInitialized.AddListener(AttributeInitialized);
            Ignore(owner.gameObject);
        }

        private void AttributeInitialized(EntityAttribute attributeInOwner)
        {
            for (int i = 0; i < this.attributesThatInfluenceDamage.Count || i < this.attributesThatInfluenceImpact.Count; i++)
            {
                if (i < this.attributesThatInfluenceDamage.Count
                    && this.attributesThatInfluenceDamage[i].Attribute.Alias.Equals(attributeInOwner.Alias))
                {
                    this.attributesThatInfluenceDamage[i].Attribute = attributeInOwner; //replace default scriptable object with instance in owner
                }
                if (i < this.attributesThatInfluenceImpact.Count
                    && this.attributesThatInfluenceImpact[i].Attribute.Alias.Equals(attributeInOwner.Alias))
                {
                    this.attributesThatInfluenceImpact[i].Attribute = attributeInOwner; //replace default scriptable object with instance in owner
                }
            }
        }

        protected override bool ShouldTrigger(Collider other)
        {
            bool result = base.ShouldTrigger(other);
            if (result && this.ignoreOthersFromOwner)
            {
                var handler = other.transform.GetComponentInParentsOrChildren<TouchDamageHandler>();
                result = !(handler && handler.owner == owner);
            }

            return result;
        }

        private void DoCollide(GameObject collidedObject)
        {
            Collider other = collidedObject.GetComponent<Collider>();
            if (collidersToIgnore != null && collidersToIgnore.Contains(other))
                return;

            IInteractable interactable = collidedObject.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                if (interactable is Entity)
                {
                    CharController controller = collidedObject.GetComponent<CharController>();
                    if (controller)
                        controller.Hit();
                }

                //float strBonus = str ? str.Value * 0.5f : 0;
                AttackAction attack = owner.GetComponent<AttackAction>();

                if (attack)
                    attack.Execute(GetAttributesInfluence(this.attributesThatInfluenceDamage), interactable);
            }
            SimulateImpact(collidedObject, impactMultiplier, true);
        }

        private float GetAttributesInfluence(List<EntityAttributeInfluence> listOfAttributes)
        {
            float bonus = 0;

            if (listOfAttributes != null)
            {
                foreach (var influence in listOfAttributes)
                {
                    bonus += influence.CalculateInfluencedValue(bonus);
                }
            }

            return bonus;
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

                // float strModifier = str ? str.Value : 1;
                float impactModifier = 1 + GetAttributesInfluence(this.attributesThatInfluenceImpact);
                body.AddForce((dir * multiplier.x + Vector3.up * multiplier.y) * impactModifier, modeOnImpact);
            }
        }
    }
}