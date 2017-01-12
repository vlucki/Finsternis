namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using Extensions;
    using System;
    using Random = UnityEngine.Random;
    using System.Linq;

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
            float value = ComputeConstraints(effect) * GetEffectRarity(effect);

            return value;
        }

        private float GetEffectRarity(Effect effect)
        {
            AttributeModifier modifier = effect as AttributeModifier;
            float value = 0;
            if (modifier)
            {
                float multiplier = modifier.ValueChange;
                switch (modifier.TypeOfModifier)
                {
                    case AttributeModifier.ModifierType.INCREASE:
                        value = (modifier.TypeOfChange == AttributeModifier.ChangeType.ABSOLUTE ? .1f : .3f) * multiplier;
                        break;
                    case AttributeModifier.ModifierType.DECREASE:
                        value = -(modifier.TypeOfChange == AttributeModifier.ChangeType.ABSOLUTE ? .1f : .3f) * multiplier;
                        break;
                    default:
                        return 1;
                }
            }
            return value;
        }

        private float ComputeConstraints(Effect effect)
        {
            float value = 1 / Mathf.Max(1, effect.Constraints.Count);
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
    }
}