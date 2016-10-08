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
        private EntityAttribute attribute;

        [SerializeField]
        private InfluenceType influenceType = InfluenceType.SUM;

        [SerializeField]
        [Tooltip("By how much the attribute value should be multiplied before calculating its influence.")]
        private float attributeMultiplier = 1;

        public EntityAttribute Attribute
        {
            get { return this.attribute; }
            set { this.attribute = value; }
        }

        public float CalculateInfluencedValue(float valueToInfluece)
        {
            float baseInfluence = this.attribute.Value * this.attributeMultiplier;

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