using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public abstract class EntityAttribute
{
    [SerializeField]
    private string _name;

    [SerializeField]
    protected List<AttributeConstraint> constraints;

    public string Name { get { return _name; } }

    protected EntityAttribute(string name)
    {
        _name = name;
        constraints = new List<AttributeConstraint>();
    }

    public AttributeConstraint GetConstraintByName(string name)
    {
        return constraints.Find(constraint => constraint.Name == name);
    }

    public List<AttributeConstraint> GetConstraintsByName(string name)
    {
        List<AttributeConstraint> result = constraints.FindAll(constraint => constraint.Name == name);
        return result;
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

    public void RemoveConstraintByName(string name)
    {
        constraints.RemoveAll(constraint => constraint.Name.Equals(name));
    }

    public void RemoveConstraints<T>()
    {
        RemoveConstraintsByType(typeof(T));
    }

    public void RemoveConstraintsByType(Type t)
    {
        constraints.RemoveAll(constraint => constraint.GetType().Equals(t));
    }

    public void RemoveConstraint(AttributeConstraint toRemove)
    {
        constraints.RemoveAll(constraint => constraint.Equals(constraint));
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

[System.Serializable]
public class EntityAttribute<T> : EntityAttribute
{

    public EntityAttribute(string name) : base(name){}

    public EntityAttribute(string name, T defaultValue) : base(name)
    {
        SetValue(defaultValue);
    }

    [SerializeField]
    protected T value;

    public T Value
    {
        get { return this.value; }
    }

    public virtual bool SetValue(T newValue)
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

    public override string ToString()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder(Name).Append(" (").Append(base.ToString()).AppendLine(")");
        constraints.ForEach(constraint => result.AppendLine(constraint.ToString()));

        return result.ToString();
    }
}