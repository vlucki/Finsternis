using System;
using UnityEngine;

[System.Serializable]
public class AttributeModifier : Effect
{
    private string attributeAlias;
    
    private float valueChange;

    public enum ModifierType
    {
        Absolute = 0,
        Relative = 10
    }
    
    public ModifierType ChangeType { get; private set; }

    public string AttributeAlias { get { return this.attributeAlias; } }

    public float ValueChange
    {
        get { return this.valueChange; }
        set { this.valueChange = value == 0 ? this.valueChange : value; }
    }

    public AttributeModifier(string atributeAlias, float valueChange, ModifierType modifierType = ModifierType.Absolute, EffectInteractionType effectType = EffectInteractionType.stackable) : base(effectType)
    {
        this.attributeAlias = atributeAlias;
        this.valueChange = valueChange;
        this.ChangeType = modifierType;
    }

    public override string ToString()
    {
        return base.ToString() + ", value modifier: " + ModifierString();
    }

    private string ModifierString()
    {
        string str = "";

        if (ChangeType == ModifierType.Relative)
            str += "*";

        return str + valueChange;
    }
}