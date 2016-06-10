using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public abstract class EntityAction : MonoBehaviour
{
    protected Entity agent;



    protected bool GetParameterOfType<T>(object[] parameters, out T result)
    {
        result = default(T);
        foreach (var v in parameters)
        {
            if (v != null && v.GetType().IsAssignableFrom(typeof(T)))
            {
                result = (T)v;
                return true;
            }
        }

        return false;
    }

    protected bool GetParametersOfType<T>(object[] parameters, out T[] result)
    {
        List<T> results = new List<T>(1);
        result = null;
        foreach (var v in parameters)
        {
            if (v != null && v.GetType().IsAssignableFrom(typeof(T)))
            {
                results.Add((T)v);
            }
        }
        result = results.ToArray();
        return result.Length > 0;
    }

    protected virtual void Awake()
    {
        agent = GetComponent<Entity>();
    }

    public abstract void Perform(params object[] parameters);
}
