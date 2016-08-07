using UnityEngine;
using System.Collections.Generic;
using System;

public class CardName : InitializableObject
{

    public enum NameType { PreName = 0, MainName = 1, PostName = 2 }

    private List<Effect> effects;

    public float Rarity { get; private set; }

    public List<string> prepositions;

    public List<Effect> Effects { get { return effects; } }

    public NameType Type { get; private set; }

    public void Init(NameType type, string name)
    {
        base.Init();
        this.name = name;
        this.effects = new List<Effect>();
        prepositions = new List<string>();
        this.Type = type;
    }

    public void AddEffect(Effect effect)
    {
        InitCheck();
        effects.Add(effect);
        Rarity += ComputeRarity(effect);
    }

    private float ComputeRarity(Effect effect)
    {
        float value = 1; //start at maximum
        var timeConstraint = effect.GetConstraint<TimeConstraint>();
        if(timeConstraint != null)
        {
            value -= Mathf.Clamp(0.5f/timeConstraint.Duration, 0, 0.5f);
        }

        AttributeModifier modifier = effect as AttributeModifier;

        if (modifier)
        {
            value += (modifier.ValueChange / 100); //negative modifiers decrease the rarity
        }

        return Mathf.Clamp01(value);
    }
}

