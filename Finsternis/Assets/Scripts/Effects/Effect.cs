using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Effect
{
    [SerializeField]
    private List<IConstraint> constraints;

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
        return constraints.Find((constraint) => { return constraint.GetType().Equals(t); }) != null;
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