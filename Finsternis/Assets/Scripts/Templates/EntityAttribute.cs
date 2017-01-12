namespace Finsternis
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using Extensions;

    [CreateAssetMenu(fileName = "Attribute", menuName = "Finsternis/Entities/Attribute")]
    [Serializable]
    public class EntityAttribute : ScriptableObject, IEquatable<EntityAttribute>
    {
        public enum ValueConstraint : byte
        {
            NONE = 0, //0b0000
            MIN = 1, //0b0001
            MAX = 2, //0b0010
            MIN_MAX = 3  //0b0011
        }

        private enum ModifierUpdateType
        {
            ADD = 1,
            REMOVE = 2
        }

        #region Variables
        /*********************************************************/
        /************************VARIABLES************************/
        /*********************************************************/
        [SerializeField, Tooltip("Unique Identifier of this Attribute (eg. vit, str, int)")]
        private string alias;

        [SerializeField]
        private float baseValue;
        private float calculatedValue; //obtained by applying modifiers to baseValue

        [SerializeField]
        private ValueConstraint constraints;

        [SerializeField]
        private RangeF valueBounds;

        /// <summary>
        /// Increase or decrease the attribute value. May change some constraints too (eg. increase the max value).
        /// </summary>
        private List<AttributeModifier> modifiers;

        private bool isInstance;

        public delegate void AttributeValueChange(EntityAttribute attribute);
        private event AttributeValueChange _onValueChangedEvent;
        public event AttributeValueChange onValueChanged
        {
            add
            {
                UnityEngine.Assertions.Assert.IsTrue(this.isInstance, "Cannot listen to value changes on attribute that is not an instance");
                lock(_onValueChangedEvent)
                {
                    _onValueChangedEvent += value;
                }
            }
            remove
            {
                UnityEngine.Assertions.Assert.IsTrue(this.isInstance, "Cannot stop listening to value changes on attribute that is not an instance");
                lock(_onValueChangedEvent)
                {
                    _onValueChangedEvent -= value;
                }
            }
        }
        #endregion

        #region Properties
        /**********************************************************/
        /************************PROPERTIES************************/
        /**********************************************************/
        public string Alias { get { return this.alias; } }

        public float Value
        {
            get { return this.calculatedValue; }
            set
            {
                UnityEngine.Assertions.Assert.IsTrue(this.isInstance, "Cannot set Value on attribute that is not an instance");
                float oldValue = this.calculatedValue;
                this.calculatedValue = EnforceConstraints(value);
                NotifyChanges(oldValue);
            }
        }

        public float BaseValue { get { return this.baseValue; } }

        public float Min { get { return this.valueBounds.min; } }

        public float Max { get { return this.valueBounds.max; } }
        #endregion

        #region Methods
        /*********************************************************/
        /*************************METHODS*************************/
        /*********************************************************/
        public static EntityAttribute CreateInstance()
        {
            var instance = ScriptableObject.CreateInstance<EntityAttribute>();
            instance.isInstance = true;
            return instance;
        }

        public static EntityAttribute CreateInstance(EntityAttribute source)
        {
            var instance = Instantiate(source);
            instance.isInstance = true;
            return instance;
        }

        public EntityAttribute Copy()
        {
            var instance = EntityAttribute.CreateInstance(this);
            return instance;
        }

        /// <summary>
        /// Creates a nicely formatted string to summarize the attribute.
        /// </summary>
        /// <returns>String containing the attribute's name, alias and value.</returns>
        public override string ToString()
        {
            return this.name + " (" + alias + "): " + Value.ToString("n2");
        }

        /// <summary>
        /// Calls GetHashCode on the attribute's alias.
        /// </summary>
        /// <returns>The hashcode of the attribute's alias.</returns>
        public override int GetHashCode()
        {
            int returnValue = 13;
            if (!this.alias.IsNullOrEmpty())
                returnValue = this.alias.GetHashCode();
            return returnValue;
        }

        /// <summary>
        /// <see cref="Equals(EntityAttribute)"/>
        /// </summary>
        public override bool Equals(object other)
        {
            return this.Equals(other as EntityAttribute);
        }

        /// <summary>
        /// Compares the alias of this EntityAttribute with the passed one.
        /// </summary>
        /// <param name="other">EntityAttribute to be compared.</param>
        /// <returns>True of both aliases are the same.</returns>
        public bool Equals(EntityAttribute other)
        {
            if (!other)
                return false;

            if (this.alias.IsNullOrEmpty())
                return false;

            return this.alias.Equals(other.alias);
        }

        public bool HasMaximumValue()
        {
            return (this.constraints & ValueConstraint.MAX) == ValueConstraint.MAX;
        }

        public bool HasMinimumValue()
        {
            return (this.constraints & ValueConstraint.MIN) == ValueConstraint.MIN;
        }

        public void AddModifier(AttributeModifier modifierToAdd)
        {
            UpdateModifier(modifierToAdd, ModifierUpdateType.REMOVE);
        }

        public void RemoveModifier(AttributeModifier modifierToRemove)
        {
            UpdateModifier(modifierToRemove, ModifierUpdateType.REMOVE);
        }

        public void AddModifiers(IEnumerable<AttributeModifier> modifiersToAdd)
        {
            BatchUpdateModifiers(modifiersToAdd, ModifierUpdateType.ADD);
        }

        public void RemoveModifiers(IEnumerable<AttributeModifier> modifiersToRemove)
        {
            BatchUpdateModifiers(modifiersToRemove, ModifierUpdateType.REMOVE);
        }

        /// <summary>
        /// Updates multiple modifiers at once.
        /// </summary>
        /// <param name="modifiersToUpdate">Modifiers that need to be updated on the list.</param>
        /// <param name="updateType">Whether the modifier is to be added or removed.</param>
        private void BatchUpdateModifiers(IEnumerable<AttributeModifier> modifiersToUpdate, ModifierUpdateType updateType)
        {
            float oldValue = this.Value;
            foreach (var modifier in modifiersToUpdate)
                UpdateModifier(modifier, updateType, false);
            NotifyChanges(oldValue);
        }

        /// <summary>
        /// Updates the list of modifiers and the current value of the attribute.
        /// </summary>
        /// <param name="modifierToUpdate">Modifier that needs to be updated on the list.</param>
        /// <param name="updateType">Whether the modifier is to be added or removed.</param>
        /// <param name="notifyChanges">Should the new value trigger an onValueChanged event?</param>
        private void UpdateModifier(AttributeModifier modifierToUpdate, ModifierUpdateType updateType, bool notifyChanges = true)
        {            
            UnityEngine.Assertions.Assert.IsTrue(this.isInstance, "Trying to add modifier to update modifiers in attribute that is not an instance");

            float? auxiliar = null;
            if (modifierToUpdate.TypeOfChange == AttributeModifier.ChangeType.RELATIVE)
            {
                auxiliar = this.HasMaximumValue() ? this.valueBounds.max : this.baseValue;
            }
            float newValue = modifierToUpdate.GetModifiedValue(this.Value, auxiliar);

            if (notifyChanges)
                this.Value = newValue;
            else
                this.calculatedValue = newValue; //set value directly, avoiding a call to onValueChanged

            if (updateType == ModifierUpdateType.REMOVE)
                this.modifiers.Remove(modifierToUpdate);
            else if (updateType == ModifierUpdateType.ADD)
                this.modifiers.Add(modifierToUpdate);
        }

        /// <summary>
        /// Call <paramref name="onValueChanged"/> if the current value differs from the passed one.
        /// </summary>
        /// <param name="oldValue">Value to be compared with the current one (usually, an old value)</param>
        private void NotifyChanges(float oldValue)
        {
            if (oldValue != this.calculatedValue && this._onValueChangedEvent != null)
                this._onValueChangedEvent(this);
        }
        
        /// <summary>
        /// Changes the base value of this attribute, updating the minimum and maximum if they exist.
        /// </summary>
        /// <param name="newValue">The new value of the attribute.</param>
        public void SetBaseValue(float newValue, bool updateMin = true, bool updateMax = true)
        {
            if (this.baseValue != newValue)
            {
                float oldValue = this.baseValue;
                this.baseValue = newValue;
                if (HasMinimumValue())
                {
                    if (updateMin)
                        this.valueBounds.min = Mathf.Min(this.Min, this.baseValue);
                    else
                        this.baseValue = Mathf.Max(this.Min, this.baseValue);
                }
                if (HasMaximumValue())
                {
                    if (updateMax)
                        this.valueBounds.max = Mathf.Max(this.Max, this.baseValue);
                    else
                        this.baseValue = Mathf.Min(this.Max, this.baseValue);
                }

                Value += this.baseValue - oldValue; //update actual value with the difference
            }
        }

        /// <summary>
        /// Ensures the given value is within the limits of the attribute, if there are any.
        /// </summary>
        /// <param name="rawValue">Value to be checked.</param>
        /// <returns>A value that is within the attribute bounds (if any)</returns>
        private float EnforceConstraints(float rawValue)
        {
            if (HasMinimumValue())
                rawValue = Mathf.Max(this.Min, rawValue);

            if (HasMaximumValue())
                rawValue = Mathf.Min(this.Max, rawValue);

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

            bool result = this.Min != newMin;

            this.valueBounds.min = newMin;

            if (HasMaximumValue() && this.Min > this.Max)
            {
                if (updateMax)
                    this.valueBounds.max = this.Min;
                else
                    this.valueBounds.min = this.Max;

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

            bool result = this.Max != newMax;

            this.valueBounds.max = newMax;

            if (HasMinimumValue() && this.Max < this.Min)
            {
                if (updateMin)
                    this.valueBounds.min = this.Max;
                else
                    this.valueBounds.max = this.Min;

                SetBaseValue(this.baseValue);
            }

            return result;
        }
        #endregion

#if UNITY_EDITOR
        #region EDITOR CODE

        float lastMin;
        float lastMax;
        string lastAlias;

        void OnValidate()
        {
            if (HasMinimumValue())
            {
                if (lastMin != this.Min)
                    SetMin(this.Min);
            }
            else
                this.valueBounds.min = 0;

            if (HasMaximumValue())
            {
                if (lastMax != this.Max)
                    SetMax(this.Max);
            }
            else
                this.valueBounds.max = 0;

            SetBaseValue(this.baseValue);

            if (!string.IsNullOrEmpty(this.alias) && !this.alias.Equals(lastAlias))
            {
                this.alias = this.alias.ToLower();
                lastAlias = this.alias;
            }

            lastMin = this.Min;
            lastMax = this.Max;
        }


        #endregion
#endif
    }
}