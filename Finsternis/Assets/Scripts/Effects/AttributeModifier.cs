using System;
using UnityEngine;

[System.Serializable]
public class AttributeModifier : Effect
{    
    private float valueChange;

    public enum ModifierType
    {
        Absolute = 0,
        Relative = 10
    }
    
    public ModifierType ChangeType { get; private set; }

    public string AttributeAlias { get; private set; }

    public float ValueChange
    {
        get { return this.valueChange; }
        set { this.valueChange = value == 0 ? this.valueChange : value; }
    }

    public AttributeModifier(string atributeAlias, float valueChange, ModifierType modifierType = ModifierType.Absolute, string name = null) : base(name)
    {
        this.AttributeAlias = atributeAlias;
        this.valueChange = valueChange;
        this.ChangeType = modifierType;
    }

    public override string ToString()
    {
        var str = base.ToString();

        return str.Substring(0, str.Length) 
            + ", attribute: '" + this.AttributeAlias
            + "', amount: " + StringfyValue();
    }

    public string StringfyValue()
    {
        string str = "";

        if (ChangeType == ModifierType.Relative)
            str += "x";
        else if (this.valueChange > 0)
            str += "+"; 

        return str + valueChange;
    }

    /// <summary>
    /// Compares the attribute modified and the type of modifier in question.
    /// </summary>
    /// <param name="other">The effect for comparison.</param>
    /// <returns>0 if both effects act upon the same attribute and are the same type of modifier.</returns>
    public override int CompareTo(Effect other)
    {
        int result = base.CompareTo(other);

        if (result < 1)
        {
            AttributeModifier otherModifier = other as AttributeModifier;
            if (otherModifier)
            {
                result = this.AttributeAlias.CompareTo(otherModifier.AttributeAlias);
                if (result == 0)
                {
                    result = this.ChangeType.CompareTo(otherModifier.ChangeType);
                }
            }
        }

        return result;
    }

    public override bool Merge(Effect other)
    {
        AttributeModifier otherModifier = other as AttributeModifier;
        if (otherModifier)
        {
            if(this.CompareTo(otherModifier) == 0)
            {
                this.valueChange += otherModifier.valueChange;
                return true;
            }
        }
        return false;
    }

    public override object Clone()
    {
        AttributeModifier clone = new AttributeModifier(this.AttributeAlias, this.valueChange, this.ChangeType, this.Name);
        constraints.ForEach(
            constraint => {
                ICloneable cloneable = constraint as ICloneable;
                if (cloneable != null)
                    clone.AddConstraint((IConstraint)(cloneable.Clone())); //try to make a deep copy of the constraint list
                else
                    clone.AddConstraint(constraint); //fallback, if the ICloneable interface is not implemented by the constraint
                }
            );
        return clone;
    }
}