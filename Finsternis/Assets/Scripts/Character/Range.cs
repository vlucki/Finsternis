using UnityEngine;
using System.Collections;
using System;

public class Range : AttributeConstraint
{
    float min;
    float max;

    public Range(float? min, float? max)
    {
        if (min == max && min == null)
            throw new ArgumentNullException("min and max", "Parameters 'min' and 'max' cannot both be null!");
        if (min >= max)
            throw new ArgumentOutOfRangeException();
    }

    public override bool Check()
    {
        throw new NotImplementedException();
    }
}
