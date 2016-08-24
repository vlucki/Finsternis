﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[SelectionBase]
[DisallowMultipleComponent]
public class Entity : MonoBehaviour
{
    public UnityEvent onInteraction;

    public EntityAction lastInteraction;

    [SerializeField]
    protected bool interactable = true;

    [SerializeField]
    protected List<EntityAttribute> attributes;

    public System.Collections.ObjectModel.ReadOnlyCollection<EntityAttribute> Attributes { get { return attributes.AsReadOnly(); } }

    protected virtual void Awake()
    {
        for(int i = 0; i < attributes.Count; i++)
            InitializeAttribute(i);
    }

    protected virtual void InitializeAttribute(int attributeIndex)
    {
        attributes[attributeIndex] = Instantiate(attributes[attributeIndex]);
        attributes[attributeIndex].SetOwner(this);
    }

    public EntityAttribute GetAttribute(string alias, bool createIfNotFound = false)
    {
        EntityAttribute attribute = attributes.Find(existingAttribute => existingAttribute.Alias.Equals(alias));
        if (!attribute && createIfNotFound)
        {
            attribute = ScriptableObject.CreateInstance<EntityAttribute>();
            attribute.Alias = alias;
        }

        return attribute;
    }

    public void AddAttribute(EntityAttribute attribute)
    {
        if (!attributes.Find(existingAttribute => existingAttribute.Alias.Equals(attribute.Alias)))
        {
            attributes.Add(attribute);
            attribute.SetOwner(this);
        }
    }

    public virtual void Interact(EntityAction action)
    {
        lastInteraction = action;
        if(onInteraction != null)
            onInteraction.Invoke();
    }

    public virtual void AtributeUpdated(EntityAttribute attribute)
    {
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
