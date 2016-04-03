using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class EntityAttribute
{
    private string _name;
    protected List<AttributeConstraint> constraints;

    public string Name { get { return _name; } }

    protected EntityAttribute(string name)
    {
        _name = name;
        constraints = new List<AttributeConstraint>();
    }

    public bool AddConstraint(AttributeConstraint constraint)
    {
        constraint.SetAttribute(this);
        if (!constraints.Contains(constraint))
        {
            constraints.Add(constraint);
            return true;
        }
        return false;
    }

    public void AddConstraints(AttributeConstraint[] constraints)
    {
        for (int i = 0; i < constraints.Length; i++)
            AddConstraint(constraints[i]);
    }

    protected bool CheckConstraints()
    {
        foreach (AttributeConstraint constraint in constraints)
            if (!constraint.Check())
                return false;

        return true;
    }
}

public class EntityAttribute<T> : EntityAttribute
{

    public EntityAttribute(string name) : base(name){}

    protected T value;

    public T Value
    {
        get { return this.value; }
    }

    public bool SetValue(T newValue)
    {
        T oldValue = value;
        value = newValue;
        if (!CheckConstraints())
        {
            value = oldValue;
            return false;
        }

        return true;
    }
}