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
            public AttributeTemplate attributeTemplate;
            public float influenceAmount;
        }

        [Serializable]
        public struct TypeModifierRelation
        {
            public AttackData.DamageType typeInfluenced;
            public List<Modifier> modifiers;
        }

        [SerializeField]
        private AttributeTemplate healthTemplate;
        
        [SerializeField]
        private List<TypeModifierRelation> damageModifiers;

        private Attribute healthAttribute;

        protected override void Awake()
        {
            base.Awake();
            if (!healthTemplate)
            {
                this.DestroyNow();
#if UNITY_EDITOR
                Debug.LogErrorFormat(this, "NO HEALTH TEMPLATE DEFINED FOR Attack INTERACTION!");
#endif
                return;
            }
        }

        protected override bool Interact(InteractionData data)
        {
            if (!base.ValidadeInteractionType(data))
                return false;

            AttackData atkData = data as AttackData;
            if (!atkData)
                return false;

            Attribute health = this.entity.GetAttribute(healthTemplate.Alias);
            if (!health)
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