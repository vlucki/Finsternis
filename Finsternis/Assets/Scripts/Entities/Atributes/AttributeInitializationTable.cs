namespace Finsternis
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName ="AttributeInitializationTable", menuName = "Attribute Initialization Table")]
    public class AttributeInitializationTable : ScriptableObject
    {
        [Serializable]
        public struct InfluenceRange
        {
            public float min;
            public float max;
        }

        [Serializable]
        public struct Influence
        {
            public EntityAttribute attribute;
            public InfluenceRange range;
        }

        [SerializeField]
        private List<Influence> influencedAttributes;

        public Influence GetInfluence(String attributeAlias)
        {
            return this.influencedAttributes.Find(influence => influence.attribute.Alias.Equals(attributeAlias));
        }
    }
}