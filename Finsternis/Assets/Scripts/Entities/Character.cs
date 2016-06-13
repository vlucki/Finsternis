using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Character : Entity
{
    [Space(10)]
    [Header("Character")]

    public UnityEvent onDeath;

    private RangedValueAttribute health;
    private RangedValueAttribute defense;
    private bool _dead;

    [SerializeField]
    [Range(0, 5)]
    private float _invincibiltyTime = 1f;

    [SerializeField]
    private bool invincible = false;

    public bool Dead { get { return _dead; } }

    protected override void Awake()
    {
        requiredAttributes.Add("hp");
        requiredAttributes.Add("def");

        base.Awake();

        health = GetAttribute("hp");
        defense = GetAttribute("def");
    }

    public override void AtributeUpdated(EntityAttribute attribute)
    {
        if (attribute == health)
            CheckHealth();
    }

    private void CheckHealth()
    {
        if (health.Value <= 0 && !_dead)
            Die();
    }

    private void Die()
    {
        _dead = true;
        onDeath.Invoke();
        onDeath.RemoveAllListeners();
    }

    public override void Interact (EntityAction action)
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
        if (!Dead && !invincible)
        {
            health.Subtract(Mathf.Max(0, info.Amount - defense.Value));
            if (!Dead)
                StartCoroutine(TickInvincibility(_invincibiltyTime));
        }
    }

    private IEnumerator TickInvincibility(float remainingInvincibility)
    {
        invincible = true;
        while(remainingInvincibility > 0)
        {
            remainingInvincibility -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        invincible = false;
    }
}
