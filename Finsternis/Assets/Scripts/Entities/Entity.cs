namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using System;
    using Extensions;
    using System.Collections;
    using System.Linq;
    using EasyEditor;

    [SelectionBase, DisallowMultipleComponent, RequireComponent(typeof(InteractionModule))]
    public abstract class Entity : CustomBehaviour, IEnumerable<Attribute>
    {
        #region EVENT CLASSES
        [Serializable]
        public class AttributeInitializedEvent : Events.CustomEvent<Attribute> { }
        #endregion

        #region VARIABLES
        [SerializeField]
        private new string name;

        [SerializeField, Inspector(rendererType = "InlineClassRenderer")]
        private AttributeInitializationTable initializationTable;

        /// <summary>
        /// Event invoked whenever an attribute is initialized. By default, it happens during execution of the Entity's Start method.
        /// </summary>
        public AttributeInitializedEvent onAttributeInitialized;

        protected Dictionary<string, Attribute> attributes;

        #endregion

        public Attribute this[string alias] { get { return this.attributes[alias]; } }

        protected virtual void Awake()
        {
            this.attributes = new Dictionary<string, Attribute>();
            if (!this.name.IsNullOrEmpty())
                base.name = this.name;
        }

        protected virtual void Start()
        {
            if (this.initializationTable)
            {
                foreach (var influence in this.initializationTable)
                {
                    InitializeAttribute(influence);
                }
            }
        }

        protected virtual void InitializeAttribute(Influence influence)
        {
            Attribute attribute = null;
            if (influence.template)
            {
                UnityEngine.Random.InitState(this.name.GetHashCode() + influence.template.Alias.GetHashCode());
                int value = Mathf.CeilToInt(UnityEngine.Random.Range(influence.range.min, influence.range.max));
                attribute = new Attribute(influence.template.Alias, value);

                if (influence.template.HasMinimumValue)
                    attribute.AddConstraint(new AttributeConstraint()
                    { Type = AttributeConstraint.AttributeConstraintType.MIN, Value = influence.template.Min });

                if (influence.template.HasMaximumValue)
                    attribute.AddConstraint(new AttributeConstraint()
                    { Type = AttributeConstraint.AttributeConstraintType.MAX, Value = influence.template.Max });
                
            }

            if (attribute && onAttributeInitialized && AddAttribute(attribute))
                onAttributeInitialized.Invoke(attribute);
        }

        public bool AddAttribute(Attribute attribute)
        {
            bool mayAddAttribute = !this.attributes.ContainsKey(attribute.Alias);
            if (!mayAddAttribute)
                this.attributes[attribute.Alias] = attribute;

            return mayAddAttribute;
        }

        public Attribute GetAttribute(string alias)
        {
            Attribute result = null;
            this.attributes.TryGetValue(alias, out result);

            return result;
        }

        public IEnumerator<Attribute> GetEnumerator()
        {
            return attributes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}