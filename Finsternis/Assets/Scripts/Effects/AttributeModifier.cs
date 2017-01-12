namespace Finsternis
{
    using System;
    using UnityEngine;

    [Serializable]
    public class AttributeModifier : Effect
    {
        [Serializable]
        public enum ModifierType
        {
            INCREASE = 0,
            DECREASE = 10
        }

        public enum ChangeType
        {
            ABSOLUTE = 0,
            RELATIVE = 10
        }

        public EntityAttribute ModifiedAttribute { get; protected set; }

        public float ValueChange { get; protected set; }

        public ModifierType TypeOfModifier { get; private set; }

        public ChangeType TypeOfChange { get; private set; }

        public AttributeModifier(EntityAttribute targetAttribute, ModifierType modifierType = ModifierType.INCREASE, ChangeType changeType = ChangeType.ABSOLUTE, string name = null) : base(name)
        {
            this.ModifiedAttribute = targetAttribute;
            this.TypeOfModifier = modifierType;
            this.TypeOfChange = changeType;
        }

        public override string ToString()
        {
            return base.ToString() + string.Format("\n{0} Modifier Type = {0} Change Type = {1}", name ?? "No Name", TypeOfModifier.ToString(), TypeOfChange.ToString());
        }

        public string StringfyValue()
        {
            string str;

            switch (this.TypeOfModifier)
            {
                case ModifierType.INCREASE:
                    str = "+";
                    break;
                case ModifierType.DECREASE:
                    str = "-";
                    break;
                default:
                    return null;
            }
            
            str += ValueChange.ToString("n2");

            return str;
        }

        public bool Merge(Effect other)
        {
            AttributeModifier otherModifier = other as AttributeModifier;
            if (otherModifier && otherModifier.TypeOfModifier == this.TypeOfModifier && otherModifier.TypeOfChange == this.TypeOfChange)
            {
                if (this.TypeOfChange == ChangeType.RELATIVE)
                    this.ValueChange *= otherModifier.ValueChange;
                else
                    this.ValueChange += otherModifier.ValueChange;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates a new value according to the modifier type and value.
        /// </summary>
        /// <param name="currentValue">Value to be modified.</param>
        /// <param name="auxiliarValue">Value to be used when modifient "currentValue" for Relative modifiers.</param>
        /// <returns>A new value, after applying the modifier to the passed values.</returns>
        public float GetModifiedValue(float currentValue, float? auxiliarValue = null)
        {
            float modifiedValue = currentValue;

            switch (this.TypeOfChange)
            {
                case ChangeType.ABSOLUTE:
                    modifiedValue += (this.TypeOfModifier == ModifierType.DECREASE ? -this.ValueChange : this.ValueChange);
                    break;
                case ChangeType.RELATIVE:
                    if (!auxiliarValue.HasValue)
                        throw new ArgumentException("Relative modifiers require a base to calculate the modified value.", "auxiliarValue");
                    modifiedValue += auxiliarValue.Value * (this.TypeOfModifier == ModifierType.DECREASE ? -this.ValueChange : this.ValueChange);
                    break;
            }

            return modifiedValue;
        }
    }
}