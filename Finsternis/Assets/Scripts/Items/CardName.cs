namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityQuery;
    using System;

    [CreateAssetMenu(fileName = "Card Name", menuName = "Finsternis/Cards/Card Name")]
    public class CardName : ScriptableObject
    {

        public enum NameType { PreName = 0, MainName = 1, PostName = 2 }

        [SerializeField]
        private NameType nameType;

        [SerializeField]
        private bool isStackable = true;

        [SerializeField]
        [ReadOnly]
        private float rarity;

        [SerializeField]
        private List<AttributeModifier> effects;

        [SerializeField]
        private List<string> prepositions;

        [SerializeField]
        private string flavourText;

        public List<AttributeModifier> Effects { get { return this.effects; } }

        public float Rarity { get { return this.rarity; } }

        public bool IsStackable { get { return this.isStackable; } }

        public NameType Type { get { return this.nameType; } }

        public string FlavourText { get { return this.flavourText; } }

        public int PrepositionsCount { get { return this.prepositions.Count; } }

        public string GetPreposition(int index)
        {
            return this.prepositions[index];
        }

        private float ComputeRarity(Effect effect)
        {
            //float value = 0.2f / Mathf.Max(1, effect.ConstraintsCount);
            //value = ComputeConstraints(value, effect);
            //value += ComputeAttributeModifiers( effect);

            return ComputeAttributeModifiers(effect);
        }

        private float ComputeAttributeModifiers(Effect effect)
        {
            AttributeModifier modifier = effect as AttributeModifier;
            float value = 0;
            if (modifier && !modifier.AttributeAlias.IsNullOrEmpty())
            {
                switch (modifier.TypeOfModifier)
                {
                    case AttributeModifier.ModifierType.SUM:
                        value = 0.075f;
                        break;
                    case AttributeModifier.ModifierType.SUBTRACT:
                        value = -0.075f;
                        break;
                    case AttributeModifier.ModifierType.MULTIPLY:
                        value = 0.2f;
                        break;
                    case AttributeModifier.ModifierType.DIVIDE:
                        value = -0.2f;
                        break;
                }
            }
            return value;
        }

        private float ComputeConstraints(float value, Effect effect)
        {
            var timeConstraint = effect.GetConstraint<TimeConstraint>();
            if (timeConstraint != null)
            {
                value *= 0.6f - Mathf.Clamp(0.5f / timeConstraint.Duration, 0.1f, 0.5f);
            }
            return value;
        }

        public override bool Equals(object o)
        {
            if (o == null)
                return false;

            CardName name = o as CardName;
            if (!name)
                return false;

            if (!name.name.Equals(this.name))
                return false;

            if (!name.Type.Equals(this.Type))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return Mathf.RoundToInt((this.name.GetHashCode() * 73 + this.Type.GetHashCode() * 919));
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            float rarity = 0;
            switch (this.Type)
            {
                case NameType.PreName:
                    rarity = 0.1f;
                    break;
                case NameType.PostName:
                    rarity = 0.15f;
                    break;
                case NameType.MainName:
                    this.isStackable = false;
                    break;
            }
            
            foreach (var effect in this.effects)
                rarity *= (1 + ComputeRarity(effect));

            if (this.isStackable)
                rarity *= 1.5f;

            this.rarity = Mathf.Abs(rarity);
        }
#endif
    }
}