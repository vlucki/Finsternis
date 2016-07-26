using UnityEngine;
using System.Collections.Generic;

public class CardName : ScriptableObject
{
    private List<Effect> effects;
    private float rarity;

    public float Rarity { get { return rarity; } }
}

public abstract class Effect : ScriptableObject
{
}

public abstract class AttributeEffect : Effect
{
    private string attributeAlias;
    private float valueChange;

    public string AttributeAlias { get { return attributeAlias; } }

}