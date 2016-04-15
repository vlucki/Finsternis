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

    private RARITY _rarity;

    private int _cost;

    public RARITY Rarity
    {
        get { return _rarity; }
    }

    public int Cost
    {
        get { return _cost; }
    }

    public void Init(RARITY rarity, int cost)
    {
        _rarity = rarity;
        _cost = cost;
    }
}
