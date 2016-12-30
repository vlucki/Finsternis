namespace Finsternis
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "AttributeInitializationTable", menuName = "Attribute Initialization Table")]
    public class AttributeInitializationTable : ScriptableObject
    {
        [Serializable]
        public struct InfluenceRange
        {
            [Range(.5f, 999)]
            public float min;
            [Range(.5f, 999)]
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

#if UNITY_EDITOR
        void OnValidate()
        {
            this.influencedAttributes.ForEach(influence =>
                influence.range.max = Mathf.Max(influence.range.max, influence.range.min));
        }
#endif
    }
}