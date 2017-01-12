namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using Extensions;
    using System.Collections;
    using EasyEditor;

    [SelectionBase, DisallowMultipleComponent, RequireComponent(typeof(InteractionModule))]
    public abstract class Entity : CustomBehaviour, IEnumerable<EntityAttribute>
    {
        #region EVENT CLASSES
        [System.Serializable]
        public class AttributeInitializedEvent : Events.CustomEvent<EntityAttribute> { }
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

        protected Dictionary<string, EntityAttribute> attributes;

        #endregion

        public EntityAttribute this[string alias] { get { return this.attributes[alias]; } }

        public bool AttributesInitialized { get; protected set; }

        protected virtual void Awake()
        {
            this.attributes = new Dictionary<string, EntityAttribute>();
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
            this.AttributesInitialized = true;
        }

        protected virtual void InitializeAttribute(Influence influence)
        {
            EntityAttribute attribute = null;
            if (influence.template)
            {
                attribute = influence.template.Copy();
                UnityEngine.Random.InitState(this.name.GetHashCode() + influence.template.Alias.GetHashCode());
                attribute.Value = Mathf.CeilToInt(UnityEngine.Random.Range(influence.range.min, influence.range.max));
            }

            if (attribute && onAttributeInitialized)
                onAttributeInitialized.Invoke(attribute);
        }

        public bool AddAttribute(EntityAttribute attribute)
        {
            bool mayAddAttribute = !this.attributes.ContainsKey(attribute.Alias);
            if (!mayAddAttribute)
                this.attributes[attribute.Alias] = attribute;

            return mayAddAttribute;
        }

        public EntityAttribute GetAttribute(string alias)
        {
            EntityAttribute result = null;
            this.attributes.TryGetValue(alias, out result);

            return result;
        }

        public IEnumerator<EntityAttribute> GetEnumerator()
        {
            return attributes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}