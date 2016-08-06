using UnityEngine;

[System.Serializable]
public class AttributeModifier : Effect
{
    [SerializeField]
    private string attributeAlias;

    [SerializeField]
    private float valueChange;

    public enum ModifierType
    {
        Absolute = 0,
        Relative = 10
    }

    [SerializeField]
    private ModifierType type;

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
    }
}