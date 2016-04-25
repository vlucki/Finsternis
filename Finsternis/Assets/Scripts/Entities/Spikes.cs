using UnityEngine;
using System.Collections;

public class Spikes : Entity
{

    public float damageModifierOnTouch = 2;
    public float damageModifierOnStay = -1;
    private float _baseDamage;
    protected override void Awake()
    {
        base.Awake();

        _baseDamage = damage.Value;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
            Poke(other, damageModifierOnTouch);
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger)
            Poke(other, damageModifierOnStay);
    }

    bool Poke(Collider other, float amount)
    {
        Entity c = other.GetComponentInParent<Entity>();
        if (c)
        {
            damage.SetValue(_baseDamage + amount);
            DoDamage(c);
            damage.SetValue(_baseDamage);
            return true;
        }

        return false;
    }

    public override void ReceiveDamage(DamageInfo info)
    {
        DoDamage(info.Source);
    }
}
