namespace Finsternis
{
    using System;
    using UnityEngine;

    [System.Serializable]
    public class AttributeModifier : Effect
    {
        [Serializable]
        public enum ModifierType
        {
            SUM = 0,
            SUBTRACT = 5,
            DIVIDE = 10,
            MULTIPLY = 15
        }
        
        private float valueChange;

        [SerializeField]
        private ModifierType modifierType;

        [SerializeField]
        private EntityAttribute affectedAttribute;

        public ModifierType ChangeType { get { return this.modifierType; } }

        public string AttributeAlias { get { return this.affectedAttribute.Alias; } }

        public float ValueChange
        {
            get { return this.valueChange; }
        }

        public AttributeModifier(EntityAttribute affectedAttribute, float valueChange, ModifierType modifierType = ModifierType.SUM, string name = null) : base(name)
        {
            this.affectedAttribute = affectedAttribute;
            this.modifierType = modifierType;
            SetValue(valueChange);
        }

        private void SetValue(float valueChange)
        {
            switch (this.modifierType)
            {
                case ModifierType.SUM:
                    this.valueChange = Mathf.Max(0, valueChange);
                    break;
                case ModifierType.SUBTRACT:
                    this.valueChange = Mathf.Min(0, valueChange);
                    break;
                case ModifierType.DIVIDE:
                    this.valueChange = Mathf.Min(1, Mathf.Abs(valueChange));
                    break;
                case ModifierType.MULTIPLY:
                    this.valueChange = Mathf.Max(1, valueChange);
                    break;
            }
        }

        public override string ToString()
        {
            var str = base.ToString();

            return str.Substring(0, str.Length)
                + ", attribute: '" + this.AttributeAlias
                + "', amount: " + StringfyValue();
        }

        public string StringfyValue()
        {
            string str = "";
            if (ChangeType == ModifierType.MULTIPLY || ChangeType == ModifierType.DIVIDE)
                str += "x";
            else if (this.valueChange > 0)
                str += "+";

            return str + valueChange.ToString("n2");
        }

        /// <summary>
        /// Compares the attribute modified and the type of modifier in question.
        /// </summary>
        /// <param name="other">The effect for comparison.</param>
        /// <returns>0 if both effects act upon the same attribute and are the same type of modifier.</returns>
        public override int CompareTo(Effect other)
        {
            int result = base.CompareTo(other);

            if (result < 1)
            {
                AttributeModifier otherModifier = other as AttributeModifier;
                if (otherModifier)
                {
                    result = this.AttributeAlias.CompareTo(otherModifier.AttributeAlias);
                    if (result == 0)
                    {
                        result = this.ChangeType.CompareTo(otherModifier.ChangeType);
                    }
                }
            }

            return result;
        }

        public override bool Merge(Effect other)
        {
            AttributeModifier otherModifier = other as AttributeModifier;
            if (otherModifier)
            {
                if (this.CompareTo(otherModifier) == 0)
                {
                    this.valueChange += otherModifier.valueChange;
                    return true;
                }
            }
            return false;
        }

        public override object Clone()
        {
            AttributeModifier clone = new AttributeModifier(this.affectedAttribute, this.valueChange, this.ChangeType, this.Name);
            constraints.ForEach(
                constraint =>
                {
                    ICloneable cloneable = constraint as ICloneable;
                    if (cloneable != null)
                        clone.AddConstraint((IConstraint)(cloneable.Clone())); //try to make a deep copy of the constraint list
                else
                        clone.AddConstraint(constraint); //fallback, if the ICloneable interface is not implemented by the constraint
                }
                );
            return clone;
        }
    }
}