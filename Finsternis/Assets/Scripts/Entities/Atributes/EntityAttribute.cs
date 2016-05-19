using UnityEngine;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(Entity))]
public abstract class EntityAttribute<T> : MonoBehaviour
{
    [SerializeField]
    protected T value;

    public string attributeName;

    public UnityEvent onValueChanged;

    protected virtual void Awake()
    {
    }

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