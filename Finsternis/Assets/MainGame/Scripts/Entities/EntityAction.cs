using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public abstract class EntityAction : MonoBehaviour
{
    protected Entity agent;

    public Entity Agent { get { return agent; } }

    protected virtual void Awake()
    {
        agent = GetComponent<Entity>();
    }

    protected bool GetParameter<T>(object[] parameters, out T result)
    {
        result = default(T);
        foreach (var v in parameters)
        {
            if (v != null && v is T)
            {
                result = (T)v;
                return true;
            }
        }

        return false;
    }

    protected bool GetParameters<T>(object[] parameters, out T[] result)
    {
        List<T> results = new List<T>(1);
        result = null;
        foreach (var v in parameters)
        {
            if (v != null && v is T)
            {
                results.Add((T)v);
            }
        }
        result = results.ToArray();
        return result.Length > 0;
    }

    public abstract void Perform(params object[] parameters);
}
