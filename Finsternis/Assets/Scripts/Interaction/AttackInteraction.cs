namespace Finsternis
{
    using UnityEngine;
    using System;
    using Extensions;
    using System.Collections.Generic;
    using System.Linq;

    public class AttackInteraction : Interaction
    {
        [Serializable]
        public struct Modifier
        {
            public EntityAttribute attributeTemplate;
            public float influenceAmount;
        }

        [Serializable]
        public struct TypeModifierRelation
        {
            public AttackData.DamageType typeInfluenced;
            public List<Modifier> modifiers;
        }

        [SerializeField]
        private EntityAttribute health;

        [SerializeField]
        private List<TypeModifierRelation> damageModifiers;

        protected override void Awake()
        {
            base.Awake();
            if (!health)
            {
                this.DestroyNow();
#if UNITY_EDITOR
                Debug.LogErrorFormat(this, "NO HEALTH TEMPLATE DEFINED FOR Attack INTERACTION!");
#endif
                return;
            }
            this.entity.onAttributeInitialized.AddListener(GrabHealth);
        }

        private void GrabHealth(EntityAttribute attribute)
        {
            if (attribute.Alias.Equals(this.health.Alias))
            {
                this.health = attribute;
                this.entity.onAttributeInitialized.RemoveListener(GrabHealth);
            }
        }

        protected override bool Interact(InteractionData data)
        {
            if (!base.ValidadeInteractionType(data))
                return false;

            AttackData atkData = data as AttackData;
            if (!atkData)
                return false;
            
            if (!this.health) //this shouldn't happen... ever
                return false;

            float finalDamageAmount = CalculateDamage(atkData);
            health.Value -= finalDamageAmount;

            return true;
        }

        private float CalculateDamage(AttackData damageInfo)
        {
            float damage = damageInfo.Amount;

            var modifierRelation = this.damageModifiers.FirstOrDefault(m => m.typeInfluenced == damageInfo.Type);
            if (!modifierRelation.modifiers.IsNullOrEmpty())
            {
                modifierRelation.modifiers.ForEach(m =>
                {
                    var attribute = this.entity.GetAttribute(m.attributeTemplate.Alias);
                    if (attribute)
                    {
                        damage -= m.influenceAmount * attribute.Value;
                    }
                });
            }

            return Mathf.Max(damage, .1f);
        }
    }
}