using UnityEngine;
using System.Collections.Generic;

public abstract class InitializableObject : ScriptableObject
{
    public bool Initialized { get; private set; }

    protected void Init()
    {
        Initialized = true;
    }

    public void InitCheck()
    {
        if (!Initialized)
            throw new System.InvalidOperationException("Object was not initialized.");
    }

}

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

    public void AddEffect(Effect effect)
    {
        InitCheck();
        effects.Add(effect);
    }
}

//TODO: look into adding constraints to effect (like time, location, attribute, etc)
public abstract class Effect : InitializableObject
{

}

public abstract class AttributeEffect : Effect
{
    [SerializeField]
    private string attributeAlias;
    [SerializeField]
    private float valueChange;

    public string AttributeAlias { get { InitCheck(); return this.attributeAlias; } }

    public void Init(string atributeAlias, float valueChange)
    {
        this.attributeAlias = atributeAlias;
        this.valueChange = valueChange;
        base.Init();
    }
}