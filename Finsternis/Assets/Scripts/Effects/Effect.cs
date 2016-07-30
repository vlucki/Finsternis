using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Effect
{
    [SerializeField]
    private List<IConstraint> constraints;

    public static implicit operator bool(Effect e)
    {
        return e != null;
    }

    public void AddConstraint(IConstraint constraint)
    {
        if (!constraint.AllowMultiple() && !HasConstraint(constraint.GetType()))
            return;

        this.constraints.Add(constraint);
    }

    public bool HasConstraint<T>() where T : IConstraint
    {
        return HasConstraint(typeof(T));
    }

    private bool HasConstraint(Type t)
    {
        return GetConstraint(t) != null;
    }

    public T GetConstraint<T>() where T : IConstraint
    {
        return (T)GetConstraint(typeof(T));
    }

    private IConstraint GetConstraint(Type t)
    {
        return constraints == null ? null : constraints.Find((constraint) => { return constraint.GetType().Equals(t); });
    }

    /// <summary>
    /// Should this effect be taken in consideation?
    /// </summary>
    /// <returns>True if every constraint is valid</returns>
    public bool ShouldBeActive()
    {
        return constraints.Find((constraint) => { return !constraint.IsValid(); }) == null;
    }

}