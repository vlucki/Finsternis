namespace Finsternis
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "AttributeTemplate", menuName = "Finsternis/Attribute Template")]
    [Serializable]
    public class AttributeTemplate : ScriptableObject
    {
        [Serializable]
        public class AttributeValueChangedEvent : CustomEvent<AttributeTemplate> { }

        public enum ValueConstraint
        {
            NONE = 0, //0b0000
            MIN = 1, //0b0001
            MAX = 2, //0b0010
            MIN_MAX = 3  //0b0011
        }

        [SerializeField]
        private string alias;

        [SerializeField]
        private ValueConstraint constraints;

        [SerializeField]
        private float baseValue;

        [SerializeField]
        private float min;

        [SerializeField]
        private float max;

        public string Alias { get { return this.alias; } }

        public float Value { get { return this.baseValue; } }

        public float BaseValue { get { return this.baseValue; } }

        public float Min { get { return this.min; } }

        public float Max { get { return this.max; } }

        public int IntValue { get { return (int)Value; } }

        public bool HasMaximumValue
        {
            get
            {
                return (this.constraints & ValueConstraint.MAX) == ValueConstraint.MAX;
            }
        }

        public bool HasMinimumValue
        {
            get
            {
                return (this.constraints & ValueConstraint.MIN) == ValueConstraint.MIN;
            }
        }

        /// <summary>
        /// Changes the base value of this attribute, updating the minimum and maximum if they exist.
        /// </summary>
        /// <param name="newValue">The new value of the attribute.</param>
        public void SetBaseValue(float newValue)
        {
            if (this.baseValue != newValue)
            {
                this.baseValue = newValue;
                if (HasMinimumValue)
                    this.min = Mathf.Min(this.min, this.baseValue);
                if (HasMaximumValue)
                    this.max = Mathf.Max(this.max, this.baseValue);
            }
        }
        

        private float EnforceLimits(float rawValue)
        {
            if (HasMinimumValue)
                rawValue = Mathf.Max(this.min, rawValue);

            if (HasMaximumValue)
                rawValue = Mathf.Min(this.max, rawValue);

            return rawValue;
        }

        /// <summary>
        /// Adds or remove the lower limit for the attribute value.
        /// </summary>
        /// <param name="newMin">The new value to be used.</param>
        /// <param name="updateMax">Should the maximum value be updated if the new minimum is greater than it?</param>
        /// <returns>True if the minimum value changed.</returns>
        public bool SetMin(float newMin, bool updateMax = false)
        {
            this.constraints |= ValueConstraint.MIN;

            bool result = this.min != newMin;

            this.min = newMin;

            if (HasMaximumValue && this.min > this.max)
            {
                if (updateMax)
                    this.max = this.min;
                else
                    this.min = this.max;

                SetBaseValue(this.baseValue);
            }

            return result;
        }

        /// <summary>
        /// Adds or remove the upper limit for the attribute value.
        /// </summary>
        /// <param name="newMax">The new value to be used.</param>
        /// <param name="updateMin">Should the minimum value be updated if the new maximum is smaller than it?</param>
        public bool SetMax(float newMax, bool updateMin = false)
        {
            this.constraints |= ValueConstraint.MAX;

            bool result = this.max != newMax;

            this.max = newMax;

            if (HasMinimumValue && this.max < this.min)
            {
                if (updateMin)
                    this.min = this.max;
                else
                    this.max = this.min;

                SetBaseValue(this.baseValue);
            }

            return result;
        }

        public override string ToString()
        {
            string toStr = Value.ToString();
            if (!string.IsNullOrEmpty(name))
                toStr = name + ": " + toStr;
            else
                toStr = alias + ": " + toStr;
            return toStr;
        }

        public bool TemplateOf(Attribute attribute)
        {
            return attribute != null && this.alias.Equals(attribute.Alias);
        }

        public Attribute MakeAttribute()
        {
            var attribute = new Attribute(this.alias, this.Value);
            if (this.HasMinimumValue)
                attribute.AddConstraint(new AttributeConstraint()
                {
                    Type = AttributeConstraint.AttributeConstraintType.MIN,
                    Value = this.min
                });


            if (this.HasMaximumValue)
                attribute.AddConstraint(new AttributeConstraint()
                {
                    Type = AttributeConstraint.AttributeConstraintType.MAX,
                    Value = this.max
                });

            return attribute;
        }

#if UNITY_EDITOR

        float lastMin;
        float lastMax;
        string lastAlias;

        void OnValidate()
        {
            if (HasMinimumValue || lastMin != this.min)
                SetMin(this.min);
            else if (!HasMinimumValue)
                this.min = 0;

            if (HasMaximumValue || lastMax != this.max)
                SetMax(this.max);
            else if (!HasMaximumValue)
                this.max = 0;

            SetBaseValue(this.baseValue);

            if (!string.IsNullOrEmpty(this.alias) && !this.alias.Equals(lastAlias))
            {
                this.alias = this.alias.ToLower();
                lastAlias = this.alias;
            }

            lastMin = this.min;
            lastMax = this.max;
        }
#endif
    }
}