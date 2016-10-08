using UnityEngine;
using System.Collections.Generic;
using System;
namespace Finsternis
{
    [RequireComponent(typeof(Entity))]
    public class TouchDamageHandler : Trigger
    {
        public Entity owner;
        public Vector2 impactMultiplier = Vector2.one;
        public ForceMode modeOnImpact = ForceMode.VelocityChange;
        public Vector2 executionMultiplier = Vector2.one;
        public ForceMode modeOnExecution = ForceMode.Impulse;
        public bool activeOnDeath = false;
        public bool ignoreOthersFromOwner = true;

        [SerializeField]
        private List<EntityAttributeInfluence> attributesThatInfluenceDamage;

        [SerializeField]
        private List<EntityAttributeInfluence> attributesThatInfluenceImpact;

        protected override void Awake()
        {
            base.Awake();
            if (!owner)
            {
                owner = GetComponentInParent<Entity>();
                if (!owner)
                    throw new System.InvalidOperationException("Attack handler needs an owner!");
                owner.onAttributeInitialized.AddListener(AttributeInitialized);
            }

            if (!activeOnDeath)
            {
                EntityAttribute health = owner.GetAttribute("vit") as EntityAttribute;
                if (health)
                    health.onValueChanged.AddListener(HealthChanged);
            }
            Ignore(owner.gameObject);
            onEnter.AddListener(DoCollide);
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

        void Start()
        {
            for(int i = 0; i < this.attributesThatInfluenceDamage.Count || i < this.attributesThatInfluenceImpact.Count; i++)
            {
                if(i < this.attributesThatInfluenceDamage.Count)
                {
                    var attributeInOwner = this.owner.GetAttribute(this.attributesThatInfluenceDamage[i].Attribute.Alias);
                    if (attributeInOwner)
                        this.attributesThatInfluenceDamage[i].Attribute = attributeInOwner; //replace default scriptable object with instance in owner
                }
                if (i < this.attributesThatInfluenceImpact.Count)
                {
                    var attributeInOwner = this.owner.GetAttribute(this.attributesThatInfluenceImpact[i].Attribute.Alias);
                    if (attributeInOwner)
                        this.attributesThatInfluenceImpact[i].Attribute = attributeInOwner; //replace default scriptable object with instance in owner
                }
            }
        }

        protected override bool ShouldTrigger(Collider other)
        {
            bool result = base.ShouldTrigger(other);
            if (result && ignoreOthersFromOwner)
            {
                TouchDamageHandler pAtkHandler = other.GetComponent<TouchDamageHandler>();
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

            if(listOfAttributes != null)
            {
                foreach(var influence in listOfAttributes)
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