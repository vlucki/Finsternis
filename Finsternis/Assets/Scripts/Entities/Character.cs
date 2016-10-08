﻿namespace Finsternis
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    public class Character : Entity
    {
        [Space(20)]
        [Header("Character Specific Variables")]

        public UnityEvent onDeath;

        private EntityAttribute health;
        private EntityAttribute defense;
        
        private bool dead;

        [SerializeField]
        [Range(0, 5)]
        private float invincibiltyTime = 1f;

        [SerializeField]
        private bool invincible = false;

        public bool Invincible { get { return this.invincible; } }

        public bool Dead { get { return this.dead; } }

        protected override void Start()
        {
            base.Start();
            this.health = GetAttribute("vit");
            if (!this.health)
                throw new System.InvalidOperationException("Characters must have \"vit\" attribute!");
            this.health.onValueChanged.AddListener(CheckHealth);

            this.defense = GetAttribute("def");
            if(!this.defense)
                throw new System.InvalidOperationException("Characters must have \"def\" attribute!");

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
            if (!interactable)
                return;
            base.Interact(action);
            if (!this.dead && action is AttackAction)
                ReceiveDamage(((AttackAction)action).DamageInfo);
        }

        protected virtual void ReceiveDamage(DamageInfo info)
        {
            if (this.invincible)
                return;

            this.health.Subtract(Mathf.Max(0, info.Amount - this.defense.Value));
            if (!Dead)
                StartCoroutine(_TickInvincibility(this.invincibiltyTime));
        }

        private IEnumerator _TickInvincibility(float remainingInvincibilityTime)
        {
            this.invincible = true;
            while (remainingInvincibilityTime > 0)
            {
                remainingInvincibilityTime -= Time.deltaTime;
                yield return null;
            }
            this.invincible = false;
        }
    }
}