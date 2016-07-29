using UnityEngine;
using System.Collections.Generic;
using System;

public class CardName : InitializableObject
{
    private List<Effect> effects;
    private float rarity;

    public float Rarity { get { return rarity; } }

    public List<Effect> Effects { get { return effects; } }

    public void Init(float rarity)
    {
        this.rarity = rarity;
        effects = new List<Effect>();
        base.Init();
    }

    public void AddEffect(AttributeModifier effect)
    {
        InitCheck();
        effects.Add(effect);
    }
}

