using UnityEngine;

[System.Serializable]
public class RangedValueAttribute : EntityAttribute<float>
{
    [SerializeField]
    [Range(0, 999)]
    private float _min = 0;
    [SerializeField]
    [Range(0, 999)]
    private float _max = 999;

    public float Min { get { return _min; } }
    public float Max { get { return _max; } }

    public int IntValue { get { return (int)Value; } }

    public override bool SetValue(float newValue)
    {
        return base.SetValue(Mathf.Clamp(newValue, _min, _max));
    }

    public void SetMax(float max, bool pushMin = false)
    {
        if (!pushMin)
            _max = Mathf.Max(max, _min);
        else
        {
            _max = max;
            _min = Mathf.Min(_min, _max);
        }
        SetValue(value);
    }

    public void SetMin(float min, bool pushMax = false)
    {
        if (!pushMax)
            _min = Mathf.Min(min, _max);
        else
        {
            _min = min;
            _max = Mathf.Max(_min, _max);
        }
        SetValue(value);
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

