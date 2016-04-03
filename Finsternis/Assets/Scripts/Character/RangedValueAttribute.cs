using UnityEngine;

public class RangedValueAttribute : EntityAttribute<float>
{
    private FloatValueConstraint _min, _max;

    public float Min { get { return _min.Value; } }
    public float Max { get { return _max.Value; } }

    public RangedValueAttribute(string name, float min, float max, float? defaultValue = null) : base(name)
    {
        _min = new FloatValueConstraint(min, ">=", "min");
        AddConstraint(_min);
        _max = new FloatValueConstraint(max, "<=", "max");
        AddConstraint(_max);
        SetValue(defaultValue == null ? max : (float)defaultValue);
    }
}

