namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using Extensions;
    using System;
    using Random = UnityEngine.Random;

    [CreateAssetMenu(fileName = "Card Name", menuName = "Finsternis/Cards/Card Name")]
    public class CardName : ScriptableObject
    {

        public enum NameType { PreName = 0, MainName = 1, PostName = 2 }

        [SerializeField]
        private NameType nameType;

        [SerializeField]
        private bool isStackable = false;

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
            float value = ComputeConstraints(effect) * ComputeAttributeModifiers(effect);

            return value;
        }

        private float ComputeAttributeModifiers(Effect effect)
        {
            AttributeModifier modifier = effect as AttributeModifier;
            float value = 0;
            if (modifier && !modifier.AttributeAlias.IsNullOrEmpty())
            {
                float multiplier = modifier.ValueChange / Mathf.Max(modifier.ValueChangeVariation.max, 1);
                switch (modifier.TypeOfModifier)
                {
                    case AttributeModifier.ModifierType.SUM:
                        value = 0.1f * multiplier;
                        break;
                    case AttributeModifier.ModifierType.SUBTRACT:
                        value = -0.1f * multiplier;
                        break;
                    case AttributeModifier.ModifierType.MULTIPLY:
                        value = 0.3f * multiplier;
                        break;
                    case AttributeModifier.ModifierType.DIVIDE:
                        value = -0.3f * multiplier;
                        break;
                }
                if (modifier.AffectedAttribute.HasMaximumValue)
                {
                    value *= 1 + modifier.ValueChange / modifier.AffectedAttribute.Max;
                }
            }
            return value;
        }

        private float ComputeConstraints(Effect effect)
        {
            float value = 1 / Mathf.Max(1, effect.ConstraintsCount);
            var timeConstraint = effect.GetConstraint<TimeConstraint>();
            if (timeConstraint != null)
            {
                value *= 0.4f - Mathf.Clamp(0.1f * timeConstraint.Duration, 0.1f, 0.39f);
            }
            return value;
        }

        public override string ToString()
        {
            return this.name;
        }

        public string ToString(int prepositionIndex)
        {
            string s = "";
            if (this.PrepositionsCount > 0)
            {
                if (prepositionIndex >= 0)
                    s = GetPreposition(prepositionIndex);
                else
                    s = this.prepositions.GetRandom(Random.Range);
                s += " ";
            }
            s += this.name;

            return s;
        }

#if UNITY_EDITOR

        public bool randomizeRange;
        void OnValidate()
        {

            float multiplier = 1;
            float exponent = .4f;

            switch (this.Type)
            {
                case NameType.PreName:
                    multiplier = 1.01f;
                    exponent = .25f;
                    break;
                case NameType.PostName:
                    multiplier = 1.02f;
                    exponent = .1f;
                    break;
                case NameType.MainName:
                    this.isStackable = false;
                    break;
            }

            foreach (var effect in this.effects)
            {
                effect.SetRange(effect.ValueChangeVariation.min, effect.ValueChangeVariation.max);

            }

            float rarity = 0.01f * multiplier;

            int[] effectsOfEachType = new int[4];



            foreach (var effect in this.effects)
            {
                if (randomizeRange)
                    RandomizeRange(effect);
                float computedRarity = ComputeRarity(effect);
                effectsOfEachType[(int)(effect.TypeOfModifier) / 10]++;
                rarity *= (1 + computedRarity) * multiplier;
                effect.UpdateName();
            }

            rarity += Mathf.Pow(.5f, (effectsOfEachType[(int)AttributeModifier.ModifierType.SUM / 10]
                        + effectsOfEachType[(int)AttributeModifier.ModifierType.MULTIPLY / 10]) / 7 * .9f);


            rarity -= Mathf.Pow(.5f, (effectsOfEachType[(int)AttributeModifier.ModifierType.SUBTRACT / 10]
                        + effectsOfEachType[(int)AttributeModifier.ModifierType.DIVIDE / 10]) / 7 * .9f);

            if (this.isStackable)
                rarity *= -1 - Mathf.Log10(rarity);

            this.rarity = Mathf.Clamp(rarity.Abs().Pow(exponent), .001f, 1);


        }

        private void RandomizeRange(AttributeModifier effect)
        {
            Random.InitState(effect.TypeOfModifier.GetHashCode() ^ effect.AttributeAlias.GetHashCode() ^ effect.AffectedAttribute.name.GetHashCode() ^ this.name.GetHashCode());
            float min = Random.Range(.5f, 10);
            if (effect.TypeOfModifier > AttributeModifier.ModifierType.SUBTRACT)
            {
                min = Random.Range(.01f, 3);
            }
            min = GetValue(min);
            var max = GetValue(min * Random.Range(1.5f, 4f));
            effect.SetRange(min, GetValue(Random.Range(min, max)));
        }

        public float GetValue(float rawValue)
        {
            int intValue = (int)rawValue;
            int remainder = (int)((rawValue - intValue) * 10) % 10;

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


            return intValue + (float)remainder / 10;
        }
#endif
    }
}