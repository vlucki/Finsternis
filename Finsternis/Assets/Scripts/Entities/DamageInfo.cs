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
    private float _amount;
    private Entity _source;

    public DamageType Type { get { return _type; } }
    public float Amount { get { return _amount; } }
    public int intAmount { get { return (int)_amount; } }
    public Entity Source { get { return _source; } }

    public DamageInfo(DamageType type, float amount, Entity source)
    {
        _type = type;
        _amount = amount;
        _source = source;
    }

}
