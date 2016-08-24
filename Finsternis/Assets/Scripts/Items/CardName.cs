using UnityEngine;
using System.Collections.Generic;
using System;

public class CardName : ScriptableObject
{

    public enum NameType { PreName = 0, MainName = 1, PostName = 2 }

    public bool IsStackable { get; private set; }

    private List<Effect> effects;

    public float Rarity { get; private set; }

    public List<string> prepositions;

    public List<Effect> Effects { get { return effects; } }

    public NameType Type { get; private set; }

    public void Init(string name, NameType type, bool stackable = true)
    {        
        this.name = name;
        this.Type = type;
        this.IsStackable = stackable; 
        this.effects = new List<Effect>();
        this.prepositions = new List<string>();
    }

    public void AddEffect(Effect effect)
    {
        UnityEngine.Assertions.Assert.IsNotNull(this.effects);

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

    public override bool Equals(object o)
    {
        if (o == null)
            return false;

        CardName name = o as CardName;
        if (!name)
            return false;

        if (!name.name.Equals(this.name))
            return false;

        if (!name.Type.Equals(this.Type))
            return false;

        if (name.Rarity != this.Rarity)
            return false;

        return true;
    }

    public override int GetHashCode()
    {
        return Mathf.RoundToInt((1+this.Rarity) * (this.name.GetHashCode() + this.Type.GetHashCode()));
    }
}

