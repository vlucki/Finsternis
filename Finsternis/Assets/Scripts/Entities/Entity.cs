using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class Entity : MonoBehaviour
{
    public UnityEvent onInteraction;

    public EntityAction lastInteraction;

    [SerializeField]
    protected bool interactable = true;

    [SerializeField]
    private List<EntityAttribute> attributes;

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
        if(!attributes.Find(existingAttribute => existingAttribute.Alias.Equals(attribute.Alias)))
            attributes.Add(attribute);
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
