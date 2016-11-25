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
            SUBTRACT = 10,
            DIVIDE = 20,
            MULTIPLY = 30
        }

        [Serializable]
        public struct RangeF
        {
            [Range(0, 30)]
            public float min;
            
            [Range(0, 30)]
            public float max;

            public RangeF(float min, float max)
            {
                this.min = min;
                this.max = Mathf.Max(min, max);
            }
        }

        [SerializeField]
        private EntityAttribute affectedAttribute;

        [Space(5)]
        [SerializeField]
        private ModifierType modifierType;

        [SerializeField]
        private RangeF valueChangeVariation;

        [SerializeField]
        [ReadOnly]
        private float valueChange;

        public RangeF ValueChangeVariation { get { return this.valueChangeVariation; } }

        public ModifierType TypeOfModifier { get { return this.modifierType; } }

        public string AttributeAlias
        {
            get
            {
                return this.affectedAttribute ? this.affectedAttribute.Alias : null;
            }
        }

        internal void SetRange(object v)
        {
            throw new NotImplementedException();
        }

        public EntityAttribute AffectedAttribute { get { return this.affectedAttribute; } }

        public float ValueChange
        {
            get { return this.valueChange; }
        }

        public AttributeModifier(EntityAttribute affectedAttribute, ModifierType modifierType = ModifierType.SUM, string name = null) : base(name)
        {
            this.affectedAttribute = affectedAttribute;
            this.modifierType = modifierType;
            UpdateName();
        }

        public void UpdateName()
        {
            if (this.affectedAttribute)
                this.name = affectedAttribute.name;
            this.name += ((modifierType == ModifierType.SUM || modifierType == ModifierType.MULTIPLY) ? " buff" : " debuff");
        }

        public void SetRange(float min, float max)
        {
            this.valueChangeVariation = new RangeF(min, max);
            CalculateValue();
        }

        public void CalculateValue()
        {
            this.valueChange = UnityEngine.Random.Range(this.valueChangeVariation.min, this.valueChangeVariation.max + .1f);
            int intValue = (int)this.valueChange;
            int remainder = (int)((this.valueChange - intValue) * 10) % 10;
            if (this.TypeOfModifier <= AttributeModifier.ModifierType.SUBTRACT)
            {
                if (remainder - 5 > 7)
                {
                    intValue++;
                    remainder = 0;
                }
                else if (remainder - 5 > -2)
                {
                    remainder = 5;
                }
                else
                {
                    remainder = 0;
                }
            }

            this.valueChange = intValue + (float)remainder / 10;
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
            string str;

            switch (this.modifierType)
            {
                case ModifierType.SUM:
                    str = "+";
                    break;
                case ModifierType.SUBTRACT:
                    str = "-";
                    break;
                case ModifierType.DIVIDE:
                case ModifierType.MULTIPLY:
                    str = "x";
                    break;
                default:
                    return null;
            }
            

            if (this.modifierType <= ModifierType.SUBTRACT)
                str += ValueChange.ToString("n2");
            else if (this.modifierType == ModifierType.DIVIDE)
            {
                str += (1 / ValueChange).ToString("n2");
            }
            else
            {
                str += ValueChange.ToString("n2");
            }

            return str;
        }

        public bool Merge(Effect other)
        {
            AttributeModifier otherModifier = other as AttributeModifier;
            if (otherModifier && otherModifier.AttributeAlias.Equals(this.AttributeAlias) && otherModifier.TypeOfModifier == this.TypeOfModifier)
            {
                if (this.modifierType > ModifierType.SUBTRACT)
                    this.valueChange *= otherModifier.valueChange;
                else
                    this.valueChange += otherModifier.valueChange;
                return true;
            }
            return false;
        }
    }
}