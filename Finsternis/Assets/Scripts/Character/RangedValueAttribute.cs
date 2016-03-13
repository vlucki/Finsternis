using UnityEngine;
using System.Collections;

public class RangedValueAttribute : EntityAttribute<float>
{
    protected float min;
    protected float max;

    public virtual float Min
    {
        get { return min; }
        set { min = value; }
    }

    public virtual float Max
    {
        get { return max; }
        set { max = value; }
    }

    public void Init(float min, float max, float current)
    {
        this.min = min;
        this.max = max;
        this.value = current;
    }
}
