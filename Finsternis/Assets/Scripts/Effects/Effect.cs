using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Effect : IComparable<Effect>, ICloneable
{
    [SerializeField]
    protected List<IConstraint> constraints;

    public string Name { get; protected set; }

    public Effect(string name = null)
    {
        this.constraints = new List<IConstraint>();
        if (!string.IsNullOrEmpty(name))
            this.Name = name;
    }

    public static implicit operator bool(Effect e)
    {
        return e != null;
    }

    public void AddConstraint(IConstraint constraint)
    {
        if (!constraint.AllowMultiple() && HasConstraint(constraint.GetType()))
            return;

        constraints.Add(constraint);
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

    public override string ToString()
    {
        return base.ToString() + ((constraints != null && constraints.Count > 0) ? ", constraints: {" + StringfyConstraints() + "}" : "");
    }

    private string StringfyConstraints()
    {
        if (this.constraints.Count == 0)
            return null;

        string constraintsStr = null;

        this.constraints.ForEach(constraint => constraintsStr += (constraint.ToString() + ","));

        return constraintsStr.Substring(0, constraintsStr.Length-2);
    }

    //Simply check if other effect is null
    public virtual int CompareTo(Effect other)
    {
        return other ? 0 : 1;
    }

    public abstract bool Merge(Effect other);

    public abstract object Clone();
}