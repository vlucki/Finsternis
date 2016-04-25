using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class AttributeTable
{
    [SerializeField]
    private List<EntityAttribute> _attributes = new List<EntityAttribute>();

    public List<EntityAttribute> Attributes { get { return _attributes; } }

    public AttributeTable(params EntityAttribute[] attributes)
    {
        if (attributes != null)
            this._attributes.AddRange(attributes);
    }

    public void AddAttribute<T>(string name, T defaultValue, params AttributeConstraint[] constraints)
    {
        EntityAttribute<T> a = new EntityAttribute<T>(name);
        if (constraints != null)
            a.AddConstraints(constraints);
        if (!a.SetValue(defaultValue))
            throw new ArgumentException("Invalid default value (" + defaultValue + ") for attribute " + a);
        AddAttribute(a);
    }

    public void AddAttribute(EntityAttribute att)
    {
        if (_attributes.FindIndex(attribute => attribute.Name.Equals(att.Name)) == -1)
            _attributes.Add(att);
    }

    public EntityAttribute<T> GetAttribute<T>()
    {
        return _attributes.Find(attribute => attribute.GetType().GetGenericTypeDefinition().Equals(typeof(T))) as EntityAttribute<T>;
    }

    public List<EntityAttribute<T>> GetAttributes<T>()
    {
        return _attributes.FindAll(attribute => attribute.GetType().GetGenericTypeDefinition().Equals(typeof(T))) as List<EntityAttribute<T>>;
    }

    public EntityAttribute GetAttribute(string name)
    {
        return _attributes.Find(attribute => attribute.Name.Equals(name));
    }

    public EntityAttribute this[string name]
    {
        get { return GetAttribute(name); }
    }
}
