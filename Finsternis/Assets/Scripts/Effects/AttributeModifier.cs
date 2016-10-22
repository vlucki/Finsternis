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
            SUM = 0,
            SUBTRACT = 5,
            DIVIDE = 10,
            MULTIPLY = 15
        }
        
        [SerializeField][ReadOnly]
        private float valueChange;

        [Space(5)]
        [SerializeField]
        private ModifierType modifierType;

        [Space(1)]
        [SerializeField]
        private EntityAttribute affectedAttribute;

        public ModifierType TypeOfModifier { get { return this.modifierType; } }
        
        public string AttributeAlias {
            get {
                return this.affectedAttribute ? this.affectedAttribute.Alias : null;
            } }

        public float ValueChange
        {
            get { return this.valueChange; }
        }

        public AttributeModifier(EntityAttribute affectedAttribute, float valueChange, ModifierType modifierType = ModifierType.SUM, string name = null) : base(name)
        {
            this.affectedAttribute = affectedAttribute;
            this.modifierType = modifierType;
            this.valueChange = valueChange;
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

            switch (this.modifierType)
            {
                case ModifierType.SUM:
                    str += "+";
                    break;
                case ModifierType.SUBTRACT:
                    str += "-";
                    break;
                case ModifierType.DIVIDE:
                    str += "/";
                    break;
                case ModifierType.MULTIPLY:
                    str += "x";
                    break;
            }

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
                        result = this.TypeOfModifier.CompareTo(otherModifier.TypeOfModifier);
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
            AttributeModifier clone = new AttributeModifier(this.affectedAttribute, this.valueChange, this.TypeOfModifier, this.Name);
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