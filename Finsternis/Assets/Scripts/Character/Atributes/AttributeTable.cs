using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class AttributeTable
{
    [SerializeField]
    private List<EntityAttribute> attributes = new List<EntityAttribute>();
    //private Dictionary<string, EntityAttribute> attributes = new Dictionary<string, EntityAttribute>();

    public List<EntityAttribute> Attributes { get { return attributes; } }

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
        if (attributes.FindIndex(attribute => attribute.Name.Equals(att.Name)) == -1)
            attributes.Add(att);
    }

    public EntityAttribute<T> GetAttribute<T>()
    {
        return attributes.Find(attribute => attribute.GetType().GetGenericTypeDefinition().Equals(typeof(T))) as EntityAttribute<T>;
    }

    public List<EntityAttribute<T>> GetAttributes<T>()
    {
        return attributes.FindAll(attribute => attribute.GetType().GetGenericTypeDefinition().Equals(typeof(T))) as List<EntityAttribute<T>>;
    }

    public EntityAttribute GetAttribute(string name)
    {
        return attributes.Find(attribute => attribute.Name.Equals(name));
    }
}
