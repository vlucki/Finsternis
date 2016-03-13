using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AttributeTable : MonoBehaviour
{
    private Dictionary<string, EntityAttribute> attributes;

    public void AddAttribute<T>(List<AttributeConstraint> constraints = null)
    {
        EntityAttribute<T> a = ScriptableObject.CreateInstance<EntityAttribute<T>>();
        if (constraints != null)
            a.AddConstraints(constraints);
    }

    public void AddAttribute(EntityAttribute att)
    {

    }

    public void AddAttribute<T>(string name, float? min = null, float? max = null)
    {
        EntityAttribute<T> a = ScriptableObject.CreateInstance<EntityAttribute<T>>();
        if (min != null)
        {

        }


        if (!attributes.ContainsKey(name))
        {
            attributes.Add(name, a);
        }
    }

    public EntityAttribute<T> GetAttribute<T>()
    {
        foreach (EntityAttribute a in attributes.Values)
        {
            if (typeof(T).Equals(a.GetType().GetGenericArguments()[0]))
            {
                return a as EntityAttribute<T>;
            }
        }
        return null;
    }

    public List<EntityAttribute<T>> GetAttributes<T>()
    {
        List<EntityAttribute<T>> foundAttributes = new List<EntityAttribute<T>>();
        foreach (EntityAttribute a in attributes.Values)
        {
            if (typeof(T).Equals(a.GetType().GetGenericArguments()[0]))
            {
                foundAttributes.Add((EntityAttribute<T>)a);
            }
        }
        return foundAttributes;
    }

    public EntityAttribute GetAttribute(string name)
    {
        return attributes[name];
    }
}
