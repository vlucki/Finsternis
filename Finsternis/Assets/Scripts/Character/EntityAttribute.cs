using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class EntityAttribute : ScriptableObject
{
    protected List<AttributeConstraint> constraints;

    protected EntityAttribute()
    {
        constraints = new List<AttributeConstraint>();
    }

    public bool AddConstraint(AttributeConstraint constraint)
    {
        if (!constraints.Contains(constraint))
        {
            constraints.Add(constraint);
            return true;
        }
        return false;
    }

    public void AddConstraints(List<AttributeConstraint> constraints)
    {
        foreach(AttributeConstraint constraint in constraints)
        {
            AddConstraint(constraint);
        }
    }
}

public class EntityAttribute<T> : EntityAttribute
{

    public EntityAttribute() : base(){}

    protected T value;

    public virtual T Value
    {
        get { return this.value; }
        set { this.value = value; }
    }
}
