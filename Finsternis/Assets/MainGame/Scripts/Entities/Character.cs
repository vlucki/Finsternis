using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Character : Entity
{
    [Space(10)]
    [Header("Character")]

    public UnityEvent onDeath;

    private RangedValueAttribute _health;
    private RangedValueAttribute _defense;

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
        if (!_dead && attribute.Value <= 0 && attribute.AttributeName.Equals("hp"))
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
        if (!Dead && !_invincible)
        {
            _health.Subtract(Mathf.Max(0, info.Amount - _defense.Value));
            if (!Dead)
                StartCoroutine(TickInvincibility(_invincibiltyTime));
        }
    }

    private IEnumerator TickInvincibility(float remainingInvincibility)
    {
        _invincible = true;
        while(remainingInvincibility > 0)
        {
            remainingInvincibility -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _invincible = false;
    }
}
