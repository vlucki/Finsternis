using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Effect
{
    /// <summary>
    /// How does this effect interact with another of the same type?
    /// </summary>
    public enum EffectInteractionType
    {
        stackable  = 0, //new instances are simply added regardless of others
        overwrite  = 1, //each new instance that is added deletes the previous one
        standalone = 2  //once it is added, any attempt to add another instance is ignored
    }
    [SerializeField]
    private List<IConstraint> constraints;

    public EffectInteractionType InteractionType { get; private set; }

    public static implicit operator bool(Effect e)
    {
        return e != null;
    }

    public Effect(EffectInteractionType interactionType = EffectInteractionType.stackable)
    {
        this.InteractionType = interactionType;
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

    public override string ToString()
    {
        return "Effect: " + InteractionType.ToString() + ", " + constraints;
    }
}