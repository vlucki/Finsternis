using UnityEngine;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(Entity))]
public abstract class EntityAttribute : MonoBehaviour
{
    public UnityEvent<EntityAttribute> onValueChanged;

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
            onValueChanged.AddListener(GetComponent<Entity>().AtributeUpdated);
        }
    }

    public virtual bool SetValue(float newValue)
    {
        value = newValue;

        if (onValueChanged != null)
            onValueChanged.Invoke(this);

        return true;
    }

    public string AttributeName
    {
        get { return attributeName; }
        set { if (String.IsNullOrEmpty(attributeName)) attributeName = value; }
    }
}