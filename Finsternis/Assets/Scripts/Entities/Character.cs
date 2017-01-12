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
        private EntityAttribute health;
        [SerializeField]
        private EntityAttribute defense;

        public UnityEvent onDeath;

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
                this.health = AddAttributeManually("hp", 10, 0, 10);
            }


            this.health.onValueChanged += CheckHealth;

            if (!this.defense)
            {

#if UNITY_EDITOR
                Debug.LogErrorFormat(this, "Characters must have \"defense\"! Adding attribute manually");
#endif
                //(this.defenseTemplate.Alias, this.defenseTemplate.Value);
                this.defense = AddAttributeManually("def", 1, 0, null);
            }
        }

        private EntityAttribute AddAttributeManually(string alias, float fallbackBaseValue, float? fallbackMinValue, float? fallbackMaxValue)
        {
            EntityAttribute attribute = EntityAttribute.CreateInstance<EntityAttribute>();
            if (fallbackMinValue.HasValue)
                attribute.SetMin(fallbackMinValue.Value);

            if (fallbackMaxValue.HasValue)
                attribute.SetMax(fallbackMaxValue.Value);
            
            AddAttribute(attribute);
            return attribute;
        }

        protected void GrabAttributes(EntityAttribute attribute)
        {
            if (attribute.Alias == this.health.Alias)
                this.health = attribute;
            else if (attribute.Alias == this.defense.Alias)
                this.defense = attribute;
        }

        public virtual void CheckHealth(EntityAttribute health)
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