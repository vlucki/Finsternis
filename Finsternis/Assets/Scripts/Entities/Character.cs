namespace Finsternis
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;
    using Extensions;

    public class Character : Entity
    {
        [Space(20)]
        [Header("Character Specific Variables")]

        [SerializeField]
        private AttributeTemplate healthTemplate;
        [SerializeField]
        private AttributeTemplate defenseTemplate;

        public UnityEvent onDeath;

        private Attribute health;
        private Attribute defense;

        private bool dead;

        [SerializeField]
        [Range(0, 10)]
        private float invincibilityTime = 1;

        [SerializeField]
        private bool invincible = false;

        public bool Invincible { get { return this.invincible; } set { this.invincible = value; } }

        public bool Dead { get { return this.dead; } }

        protected override void Awake()
        {
            this.onAttributeInitialized.AddListener(GrabAttributes);
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            if (!this.health)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat(this, "Characters must have \"health\"! Adding attribute manually");
#endif
                this.health = AddAttributeManually(this.healthTemplate, "hp", 10, 0, 10);
            }


            this.health.valueChangedEvent += CheckHealth;

            if (!this.defense)
            {

#if UNITY_EDITOR
                Debug.LogErrorFormat(this, "Characters must have \"defense\"! Adding attribute manually");
#endif
                this.defense = new Attribute(this.defenseTemplate.Alias, this.defenseTemplate.Value);
                AddAttributeManually(this.defenseTemplate, "def", 1, 0, null);
            }
        }

        private Attribute AddAttributeManually(AttributeTemplate attributeTemplate, string alias, float baseValue, float? min, float? max)
        {
            Attribute attribute = null;
            if (attributeTemplate)
            {
                alias = attributeTemplate.Alias;
                if (attributeTemplate.HasMinimumValue)
                    min = attributeTemplate.Min;
                if (attributeTemplate.HasMaximumValue)
                    max = attributeTemplate.Max;
                baseValue = attributeTemplate.Value;
            }

#if UNITY_EDITOR
            else
                Debug.LogErrorFormat(this, "No template assigned for {0}! Adding attribute with hardcoded values - base: {1}, min: {2}, max: {3}!",
                    alias, baseValue, min, max);
#endif

            attribute = new Attribute(alias, baseValue);
            if (min != null)
                attribute.AddConstraint(new AttributeConstraint()
                {
                    Type = AttributeConstraint.AttributeConstraintType.MIN,
                    Value = (int)min
                });
            if (max != null)
                attribute.AddConstraint(new AttributeConstraint()
                {
                    Type = AttributeConstraint.AttributeConstraintType.MAX,
                    Value = (int)max
                });

            AddAttribute(attribute);
            return attribute;
        }

        protected void GrabAttributes(Attribute attribute)
        {
            if (attribute.Alias == this.healthTemplate.Alias)
                this.health = attribute;
            else if (attribute.Alias == this.defenseTemplate.Alias)
                this.defense = attribute;
        }

        public virtual void CheckHealth(Attribute health)
        {
            if (!this.dead && health.Value <= 0)
                Die();
        }

        protected virtual void Die()
        {
            this.dead = true;
            onDeath.Invoke();
            onDeath.RemoveAllListeners();
        }

        protected virtual void ReceiveDamage(AttackData info)
        {
            if (this.invincible)
                return;

            float finalDamage = info.Amount;
            switch (info.Type)
            {
                case AttackData.DamageType.physical:
                    finalDamage -= this.defense.Value * 0.9f;
                    break;
                case AttackData.DamageType.magical:
                    var inteligence = GetAttribute("int");
                    if (inteligence)
                    {
                        finalDamage -= (this.defense.Value * .2f + inteligence.Value * .5f);
                    }
                    break;

            }

            finalDamage = Mathf.Max(0, finalDamage);

            this.health.Value -= (finalDamage);
            if (!Dead)
                StartCoroutine(_TickInvincibility(this.invincibilityTime));
        }

        private IEnumerator _TickInvincibility(float remainingTime)
        {
            this.invincible = true;
            yield return WaitHelpers.Sec(remainingTime);
            this.invincible = false;
        }
    }
}