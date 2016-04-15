using UnityEngine;

[System.Serializable]
public class RangedValueAttribute : EntityAttribute<float>
{
    [SerializeField]
    private FloatValueConstraint _min;

    [SerializeField]
    private FloatValueConstraint _max;

    public float Min { get { return _min.Value; } }
    public float Max { get { return _max.Value; } }

    public int IntValue { get { return (int)Value; } }

    public void SetMin(float min)
    {
        RemoveConstraintByName("min");
        _max = new FloatValueConstraint(min, ">=", "min");
        AddConstraint(_min);
        if (Value < Min)
            SetValue(Min);
    }

    public void SetMax(float max)
    {
        RemoveConstraintByName("max");
        _max = new FloatValueConstraint(max, "<=", "max");
        AddConstraint(_max);
        if (Value > Max)
            SetValue(Max);
    }

    public RangedValueAttribute(string name, float min, float max, float? defaultValue = null) : base(name)
    {
        _min = new FloatValueConstraint(min, ">=", "min");
        AddConstraint(_min);
        _max = new FloatValueConstraint(max, "<=", "max");
        AddConstraint(_max);
        SetValue(defaultValue == null ? max : (float)defaultValue);
    }

    public override bool SetValue(float newValue)
    {
        return base.SetValue(Mathf.Clamp(newValue, Min, Max));
    }

    public void Subtract(float value)
    {
        SetValue(Value - value);
    }

    public void Add(float value)
    {
        SetValue(Value + value);
    }
}

