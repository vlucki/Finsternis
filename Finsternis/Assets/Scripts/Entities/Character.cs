using MovementEffects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Finsternis
{
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

        protected void Awake()
        {
            this.health = GetAttribute("hp", true);
            this.defense = GetAttribute("def", true);
        }

        public override void AtributeUpdated(EntityAttribute attribute)
        {
            if (!this.dead && attribute.Value <= 0 && attribute.Alias.Equals("hp"))
                Die();
        }

        private void Die()
        {
            this.dead = true;
            onDeath.Invoke();
            onDeath.RemoveAllListeners();
        }

        public override void Interact(EntityAction action)
        {
            if (interactable)
            {
                if (!Dead && typeof(AttackAction).IsAssignableFrom(action.GetType()))
                {
                    ReceiveDamage(((AttackAction)action).DamageInfo);
                }
                base.Interact(action);
            }
        }

        public virtual void ReceiveDamage(DamageInfo info)
        {
            if (!Dead && !this.invincible)
            {
                this.health -= (Mathf.Max(0, info.Amount - this.defense.Value));
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