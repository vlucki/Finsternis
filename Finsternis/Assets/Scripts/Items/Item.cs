using UnityEngine;
using System.Collections;
using System;

public class Item : ScriptableObject
{
    public enum RARITY
    {
        common,
        uncommon,
        rare,
        legendary,
        godlike
    };

    private RARITY rarity;

    private int weight;

    public RARITY Rarity
    {
        get { return rarity; }
    }

    public int Weight
    {
        get { return weight; }
    }

    public void Init(RARITY r, int weight)
    {
        this.rarity = r;
        this.weight = weight;
    }
}
