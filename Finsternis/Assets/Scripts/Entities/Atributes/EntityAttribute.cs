using UnityEngine;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(Entity))]

public abstract class EntityAttribute : MonoBehaviour
{
    [SerializeField]
    protected string attributeName;

    public string AttributeName
    {
        get { return attributeName; }
        set { if (String.IsNullOrEmpty(attributeName)) attributeName = value; }
    }
}

public abstract class EntityAttribute<T> : EntityAttribute
{
    public UnityEvent onValueChanged;

    [SerializeField]
    protected T value;

    public T Value
    {
        get { return this.value; }
    }

    public virtual bool SetValue(T newValue)
    {
        value = newValue;
        onValueChanged.Invoke();
        return true;
    }
}