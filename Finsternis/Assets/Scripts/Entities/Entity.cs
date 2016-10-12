namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using System;
    using UnityQuery;
    using System.Collections;
    using System.Linq;

    [SelectionBase]
    [DisallowMultipleComponent]
    public abstract class Entity : MonoBehaviour, IInteractable, IEnumerable<EntityAttribute>
    {
        #region inner classes and structs
        [Serializable]
        public class AttributeInitializedEvent : CustomEvent<EntityAttribute> { }

        [Serializable]
        public class EntityEvent : CustomEvent<Entity> { }

        [Serializable]
        public struct GeneralEntityEvents
        {
            public UnityEvent onInteraction;
            public EntityEvent onEnable;
            public EntityEvent onDisable;
        }
        #endregion

        #region variables
        [SerializeField]
        protected bool interactable = true;

        [SerializeField]
        protected List<InteractionConstraint> interactionConstraints;

        [SerializeField]
        private GeneralEntityEvents entityEvents;

        [SerializeField]
        protected List<EntityAttribute> attributes;

        public AttributeInitializedEvent onAttributeInitialized;
        #endregion

        public EntityAction LastInteraction { get; private set; }

        public GeneralEntityEvents EntityEvents { get { return this.entityEvents; } }

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

            if (interactionConstraints != null)
                if (interactionConstraints.Any((constraint) => !constraint.Check(action)))
                    return;

            LastInteraction = action;
            entityEvents.onInteraction.Invoke();
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

        protected virtual void OnDisable()
        {
            entityEvents.onDisable.Invoke(this);
        }

        protected virtual void OnEnable()
        {
            entityEvents.onEnable.Invoke(this);
        }

        public IEnumerator<EntityAttribute> GetEnumerator()
        {
            return attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}