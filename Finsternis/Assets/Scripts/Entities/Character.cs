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

        private EntityAttribute _health;
        private EntityAttribute _defense;

        private bool _dead;

        [SerializeField]
        [Range(0, 5)]
        private float _invincibiltyTime = 1f;

        [SerializeField]
        private bool _invincible = false;

        public bool Invincible { get { return _invincible; } }
        public bool Dead { get { return _dead; } }

        protected void Awake()
        {
            _health = CheckAttribute(_health, "hp");
            _defense = CheckAttribute(_defense, "def");
        }

        public override void AtributeUpdated(EntityAttribute attribute)
        {
            if (!_dead && attribute.Value <= 0 && attribute.Alias.Equals("hp"))
                Die();
        }

        private void Die()
        {
            _dead = true;
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
            if (!Dead && !_invincible)
            {
                _health -= (Mathf.Max(0, info.Amount - _defense.Value));
                if (!Dead)
                    Timing.RunCoroutine(_TickInvincibility(_invincibiltyTime));
            }
        }

        private IEnumerator<float> _TickInvincibility(float remainingInvincibility)
        {
            _invincible = true;
            while (remainingInvincibility > 0)
            {
                remainingInvincibility -= Time.deltaTime;
                yield return 0f;
            }
            _invincible = false;
        }
    }
}