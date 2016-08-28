namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "EntityAttribute", menuName = "Finsternis/Attribute")]
    public class EntityAttribute : ScriptableObject
    {
        [Serializable]
        public class AttributeValueChangedEvent : UnityEvent<EntityAttribute>
        {
            public static implicit operator bool(AttributeValueChangedEvent evt)
            {
                return evt != null;
            }
        }

        public enum ValueConstraint
        {
            NONE    = 0, //0b0000
            MIN     = 1, //0b0001
            MAX     = 2, //0b0010
            MIN_MAX = 3  //0b0011
        }

        [SerializeField]
        private string alias;

        [SerializeField]
        private ValueConstraint constraints;

        [SerializeField]
        private float baseValue;

        private float valueWithModifiers;

        [SerializeField]
        private float min;

        [SerializeField]
        private float max;

        [SerializeField]
        public AttributeValueChangedEvent onValueChanged;

        [SerializeField]
        [ReadOnly]
        private List<AttributeModifier> modifiers;

        public Entity Owner { get; private set; }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        internal void SetOwner(Entity entity)
        {
            if (!Owner)
            {
                Owner = entity;
            }
        }

        public string Alias
        {
            get { return this.alias; }
            set { this.alias = value; }
        }

        public float Value {
            get {
                if (this.valueWithModifiers != this.baseValue && !(modifiers != null && modifiers.Count > 0)) //make sure the value is initialized if the "SetValue" method was not called yet
                    this.valueWithModifiers = this.baseValue;
                return this.valueWithModifiers;
            }
        }

        public float BaseValue { get { return this.baseValue; } }

        public float Min
        {
            get { return this.min; }
        }

        public float Max
        {
            get { return this.max; }
        }

        public int IntValue { get { return (int)Value; } }

        public bool LimitMaximum
        {
            get
            {
                return (this.constraints & ValueConstraint.MAX) == ValueConstraint.MAX;
            }
            set
            {
                if (value != LimitMaximum)
                {
                    if (value)
                    {
                        this.constraints |= ValueConstraint.MAX;
                        this.max = this.baseValue;
                    }
                    else
                    {
                        this.constraints ^= ValueConstraint.MAX;
                        this.max = 0;
                    }
                }
            }
        }

        public bool LimitMinimum
        {
            get
            {
                return (this.constraints & ValueConstraint.MIN) == ValueConstraint.MIN;
            }
            set
            {
                if (value != LimitMinimum)
                {
                    if (value)
                    {
                        this.constraints |= ValueConstraint.MIN;
                        this.min = this.baseValue;
                    }
                    else
                    {
                        this.constraints ^= ValueConstraint.MIN;
                        this.min = 0;
                    }
                }

            }
        }

        private void RecalculateValue()
        {
            float currentValue = this.valueWithModifiers;
            this.valueWithModifiers = this.baseValue;

            if (this.modifiers != null)
            {
                modifiers.ForEach(modifier =>
                {
                    if (modifier.ChangeType == AttributeModifier.ModifierType.Absolute)
                        this.valueWithModifiers += modifier.ValueChange;
                    else
                        this.valueWithModifiers *= modifier.ValueChange;
                });
            }

            if (Value != currentValue && onValueChanged)
                onValueChanged.Invoke(this);
        }

        /// <summary>
        /// Changes the value of this attribute, respecting the minimum and maximum if they exist.
        /// </summary>
        /// <param name="newValue">The new value of the attribute.</param>
        public void SetBaseValue(float newValue)
        {
            if (LimitMinimum)
                newValue = Mathf.Max(this.min, newValue);

            if (LimitMaximum)
                newValue = Mathf.Min(this.max, newValue);

            if (this.baseValue != newValue)
            {                
                this.baseValue = newValue;
                RecalculateValue();
            }
        }

        public void AddModifier(AttributeModifier newModifier)
        {
            if (newModifier.AttributeAlias.Equals(this.alias))
            {
                if (this.modifiers == null)
                    this.modifiers = new List<AttributeModifier>();

                this.modifiers.Add(newModifier);
                RecalculateValue();
            }
        }

        public void RemoveModifier(AttributeModifier toRemove)
        {
            if (this.modifiers != null && this.modifiers.Remove(toRemove))
                RecalculateValue();
        }

        /// <summary>
        /// Adds or remove the lower limit for the attribute value.
        /// </summary>
        /// <param name="newMin">The new value to be used.</param>
        /// <param name="updateMax">Should the maximum value be updated if the new minimum is greater than it?</param>
        /// <returns>True if the minimum value changed.</returns>
        public bool SetMin(float newMin, bool updateMax = false)
        {
            LimitMinimum = true;

            bool result = this.min != newMin;

            this.min = newMin;

            if (LimitMaximum && this.min > this.max)
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
            LimitMaximum = true;

            bool result = this.max != newMax;

            this.max = newMax;

            if (LimitMinimum && this.max < this.min)
            {
                if (updateMin)
                    this.min = this.max;
                else
                    this.max = this.min;

                SetBaseValue(this.baseValue);
            }

            return result;
        }

        /// <summary>
        /// Limits the attribute value to a given range.
        /// </summary>
        /// <param name="min">The minimum value of the attribute.</param>
        /// <param name="max">The maximum value of the attribute.</param>
        /// <param name="drivingLimit">
        /// If the minimum value should push the maximum value up (0)
        /// or if the maximum value should push the minimum value down (1)
        /// </param>
        public void LimitValue(float min, float max, int drivingLimit = 0)
        {
            drivingLimit = (int)Mathf.Clamp01(drivingLimit);
            switch (drivingLimit)
            {
                case 0:
                    this.min = min;
                    this.max = Mathf.Max(min, max);
                    break;
                case 1:
                    this.max = max;
                    this.min = Mathf.Min(min, max);
                    break;
            }
        }

        /// <summary>
        /// Shorthand for the Add method.
        /// </summary>
        /// <param name="attribute">The attribute that should have its value increased.</param>
        /// <param name="amount">How much to add.</param>
        /// <returns>The attribute passed, after calling Add(amount) on it.</returns>
        public static EntityAttribute operator +(EntityAttribute attribute, float amount)
        {
            attribute.Add(amount);
            return attribute;
        }

        /// <summary>
        /// Shorthand for the Subtract method.
        /// </summary>
        /// <param name="attribute">The attribute that should have its value decreased.</param>
        /// <param name="amount">How much to subtract.</param>
        /// <returns>The attribute passed, after calling Subtract(amount) on it.</returns>
        public static EntityAttribute operator -(EntityAttribute attribute, float amount)
        {
            attribute.Subtract(amount);
            return attribute;
        }

        public void Subtract(float value)
        {
            SetBaseValue(Value - value);
        }

        public void Add(float value)
        {
            SetBaseValue(Value + value);
        }

        public override string ToString()
        {
            return alias + ": " + Value.ToString();
        }

#if UNITY_EDITOR

        float lastMin;
        float lastMax;
        string lastAlias;

        void OnValidate()
        {
            if (LimitMinimum || lastMin != this.min)
                SetMin(this.min);

            if (LimitMaximum || lastMax != this.max)
                SetMax(this.max);

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