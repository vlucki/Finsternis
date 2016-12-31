namespace Finsternis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;

    [Serializable]
    public class Attribute
    {
        private List<AttributeConstraint> constraints;
        private List<AttributeModifier> modifiers;
        private float value;

        public delegate void AttributeValueChange(Attribute attribute);

        public event AttributeValueChange valueChangedEvent;

        public string Alias { get; private set; }
        public float Value
        {
            get { return this.value; }
            set { SetValue(value); }
        }

        public ReadOnlyCollection<AttributeConstraint> Constraints { get { return this.constraints.AsReadOnly(); } }

        public ReadOnlyCollection<AttributeModifier> Modifiers { get { return this.modifiers.AsReadOnly(); } }

        public Attribute(string alias, float baseValue, params AttributeConstraint[] constraints)
        {
            this.Alias = alias;
            this.Value = baseValue;
            if (constraints.Length > 0)
                this.constraints = new List<AttributeConstraint>(constraints);
            else
                this.constraints = new List<AttributeConstraint>();
        }

        public static implicit operator bool(Attribute attribute) { return attribute != null; }

        public void SetValue(float newValue)
        {
            float oldValue = this.value;
            this.value = newValue;

            foreach (AttributeConstraint constraint in this.constraints)
            {
                if (!constraint.Validate(this.value))
                {
                    this.value = constraint.Value;
                    break;
                }
            }

            if (this.value != oldValue)
            {
                this.valueChangedEvent(this);
            }
        }

        /// <summary>
        /// Adds a new constraint to a value.
        /// If a constraint of the same type already exists, it is replaced when it is for:
        /// A) MAX VALUE and the new constraint estabilishes a higher value.
        /// B) MIN VALUE and the new constraint estabilishes a lower value. 
        /// </summary>
        /// <param name="constraintToAdd">Constraint to be added.</param>
        public void AddConstraint(AttributeConstraint constraintToAdd)
        {
            var constraint = this.constraints.Find(c => c.Type == constraintToAdd.Type);

            if (constraint != null && !constraint.AllowMultiple())
            {
                if(constraint.Type == AttributeConstraint.AttributeConstraintType.MAX && constraint.Value > constraintToAdd.Value)
                    return;

                if (constraint.Type == AttributeConstraint.AttributeConstraintType.MIN && constraint.Value < constraintToAdd.Value)
                    return;

                this.constraints.Remove(constraint);
                this.constraints.Add(constraintToAdd);
            }
            else
                this.constraints.Add(constraintToAdd);
        }

        public void RemoveConstraint(int index)
        {
            this.constraints.RemoveAt(index);
        }

        public void RemoveConstraint(AttributeConstraint constraintToRemove)
        {
            this.constraints.Remove(constraintToRemove);
        }

        public void AddModifiers(IEnumerable<AttributeModifier> modifiersToAdd)
        {
            this.modifiers.AddRange(modifiersToAdd);
            RecalculateValue();
        }

        public void AddModifier(AttributeModifier modifierToAdd)
        {
            this.modifiers.Add(modifierToAdd);
            RecalculateValue();
        }

        public void RemoveModifiers(IEnumerable<AttributeModifier> modifiersToRemove)
        {
            this.modifiers.RemoveAll(m => modifiersToRemove.Contains(m));
            RecalculateValue();
        }

        public void RemoveModifier(AttributeModifier modifierToRemove)
        {
            this.modifiers.Remove(modifierToRemove);
            RecalculateValue();
        }

        private void RecalculateValue()
        {

        }
    }
}