namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using System;

    [SelectionBase]
    [DisallowMultipleComponent]
    public abstract class Entity : MonoBehaviour, IInteractable
    {
        public UnityEvent onInteraction;

        public EntityAction lastInteraction;

        [SerializeField]
        protected bool interactable = true;

        [SerializeField]
        protected List<EntityAttribute> attributes;

        [Serializable]
        public class AttributeInitializedEvent : UnityEvent<EntityAttribute> {
            public static implicit operator bool(AttributeInitializedEvent evt)
            { return evt != null; }
        }
        public AttributeInitializedEvent onAttributeInitialized;

        public System.Collections.ObjectModel.ReadOnlyCollection<EntityAttribute> Attributes { get { return attributes.AsReadOnly(); } }

        protected virtual void Awake()
        {
            for (int i = 0; i < attributes.Count; i++)
                InitializeAttribute(i);
        }

        protected virtual void InitializeAttribute(int attributeIndex)
        {
            attributes[attributeIndex] = Instantiate(attributes[attributeIndex]);
            attributes[attributeIndex].SetOwner(this);
            if (onAttributeInitialized)
                onAttributeInitialized.Invoke(attributes[attributeIndex]);
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

        /// <summary>
        /// Interface to allow for interactions between entities
        /// </summary>
        /// <param name="action">The type of interaction that should take place (eg. Attack).</param>
        public virtual void Interact(EntityAction action)
        {
            lastInteraction = action;
            if (onInteraction != null)
                onInteraction.Invoke();
        }

        public void Kill()
        {
            Destroy(gameObject);
        }
    }
}