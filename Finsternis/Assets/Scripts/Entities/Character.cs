namespace Finsternis
{
    using MovementEffects;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class Character : Entity
    {
        [Space(20)]
        [Header("Character Specific Variables")]

        public UnityEvent onDeath;

        private EntityAttribute cachedHealth;
        private EntityAttribute cachedDefense;

        private EntityAttribute health
        {
            get { return this.cachedHealth ?? (this.cachedHealth = GetAttribute("hp", true)); }
        }

        private EntityAttribute defense
        {
            get { return this.cachedDefense ?? (this.cachedDefense = GetAttribute("def", true)); }
        }

        private bool dead;

        [SerializeField]
        [Range(0, 5)]
        private float invincibiltyTime = 1f;

        [SerializeField]
        private bool invincible = false;

        public bool Invincible { get { return this.invincible; } }

        public bool Dead { get { return this.dead; } }

        protected override void InitializeAttribute(int attributeIndex)
        {
            base.InitializeAttribute(attributeIndex);
            if (attributes[attributeIndex].Alias.Equals("hp"))
                attributes[attributeIndex].onValueChanged.AddListener(CheckHealth);
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

        public override void Interact(EntityAction action)
        {
            if (interactable)
            {
                base.Interact(action);
                if (!Dead && action is AttackAction)
                {
                    ReceiveDamage(((AttackAction)action).DamageInfo);
                }
            }
        }

        protected virtual void ReceiveDamage(DamageInfo info)
        {
            if (!this.invincible)
            {
                this.health.Subtract(Mathf.Max(0, info.Amount - this.defense.Value));
                if (!Dead)
                    Timing.RunCoroutine(_TickInvincibility(this.invincibiltyTime));
            }
        }

        private IEnumerator<float> _TickInvincibility(float remainingInvincibility)
        {
            this.invincible = true;
            while (remainingInvincibility > 0)
            {
                remainingInvincibility -= Time.deltaTime;
                yield return 0f;
            }
            this.invincible = false;
        }
    }
}