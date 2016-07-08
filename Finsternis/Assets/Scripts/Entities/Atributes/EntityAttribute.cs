using UnityEngine;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(Entity))]
public abstract class EntityAttribute : MonoBehaviour
{
    [Serializable]
    public class AttributeValueChangedEvent : UnityEvent<EntityAttribute>
    {
        public static implicit operator bool(AttributeValueChangedEvent evt)
        {
            return evt != null;
        }
    }

    public AttributeValueChangedEvent onValueChanged;

    [SerializeField]
    protected string attributeName;

    [SerializeField]
    protected float value;

    [SerializeField]
    private bool _autoNotifyEntity = true;

    public float Value
    {
        get { return value; }
    }

    void Awake()
    {
        if (_autoNotifyEntity)
        {
            Entity e = GetComponent<Entity>();

            if (!onValueChanged)
                onValueChanged = new AttributeValueChangedEvent();

            onValueChanged.AddListener(e.AtributeUpdated);
        }
    }

    public virtual bool SetValue(float newValue)
    {
        value = newValue;

        if (onValueChanged)
            onValueChanged.Invoke(this);

        return true;
    }

    public string AttributeName
    {
        get { return attributeName; }
        set { if (!String.IsNullOrEmpty(value)) attributeName = value; }
    }
}