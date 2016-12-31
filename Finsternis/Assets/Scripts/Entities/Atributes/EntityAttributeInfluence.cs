namespace Finsternis
{
    using System;
    using UnityEngine;

    [System.Serializable]
    public class EntityAttributeInfluence
    {
        public enum InfluenceType
        {
            SUM = 0,
            MULTIPLICATION = 1
        }

        public enum MaxInfluenceType
        {
            ABSOLUTE = 0,
            PERCENTAGE = 1
        }

        [SerializeField]
        private AttributeTemplate attributeTemplate;

        [SerializeField]
        private InfluenceType influenceType = InfluenceType.SUM;

        [SerializeField]
        [Tooltip("By how much the attribute value should be multiplied before calculating its influence.")]
        private float attributeMultiplier = 1;

        public AttributeTemplate AttributeTemplate
        {
            get { return this.attributeTemplate; }
        }

        public Attribute Attribute { get; set; }

        public float CalculateInfluencedValue(float valueToInfluece)
        {
            float baseInfluence = this.Attribute.Value * this.attributeMultiplier;

            valueToInfluece = CalculateInfluence(baseInfluence, valueToInfluece);

            return valueToInfluece;
        }

        private float CalculateInfluence(float baseInfluence, float valueToInfluece)
        {
            switch (this.influenceType)
            {
                case InfluenceType.SUM:
                    return valueToInfluece + baseInfluence;
                case InfluenceType.MULTIPLICATION:
                    return valueToInfluece * baseInfluence;
                default:
                    return valueToInfluece;
            }
        }
    }
}