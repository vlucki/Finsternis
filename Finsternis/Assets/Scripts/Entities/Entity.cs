namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using System;
    using UnityQuery;

    [SelectionBase]
    [DisallowMultipleComponent]
    public abstract class Entity : MonoBehaviour, IInteractable
    {
        public UnityEvent onInteraction;

        public EntityAction LastInteraction { get; private set; }

        [SerializeField]
        protected bool interactable = true;

        [SerializeField]
        protected List<EntityAttribute> attributes;

        [Serializable]
        public class AttributeInitializedEvent : CustomEvent<EntityAttribute> {
        }
        public AttributeInitializedEvent onAttributeInitialized;

        public System.Collections.ObjectModel.ReadOnlyCollection<EntityAttribute> Attributes { get { return attributes.AsReadOnly(); } }

        protected virtual void Start()
        {
            for (int i = 0; i < attributes.Count; i++)
                InitializeAttribute(i);
        }

        protected virtual void InitializeAttribute(int attributeIndex)
        {
            var attribute = Instantiate(attributes[attributeIndex]);
            attribute.name = attributes[attributeIndex].name; //remove the annoying (Clone) that Unity appends to the name
            attribute.SetOwner(this);
            if (onAttributeInitialized)
                onAttributeInitialized.Invoke(attribute);

            attributes[attributeIndex] = attribute;
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
            if (!interactable)
                return;
            LastInteraction = action;
            if (onInteraction != null)
                onInteraction.Invoke();
        }

        public void Kill()
        {
            Destroy(gameObject);
        }

        public void DisableInteraction()
        {
            this.interactable = false;
        }

        public void EnableInteraction()
        {
            this.interactable = true;
        }
    }
}