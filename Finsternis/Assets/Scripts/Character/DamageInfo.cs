using UnityEngine;
using System.Collections;

public sealed class DamageInfo
{

    public enum DamageType
    {
        physical = 1,
        magical = 2
    }

    private DamageType _type;
    private int _amount;
    private GameObject _source;

    public DamageType Type { get { return _type; } }
    public int Amount { get { return _amount; } }

    public DamageInfo(DamageType type, int amount, GameObject source)
    {
        _type = type;
        _amount = amount;
        _source = source;
    }

}
