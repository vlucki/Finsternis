namespace Finsternis
{
    using System.Collections.Generic;
    using UnityEngine;
    using Extensions;
    using System.Linq;
    using System.Collections.ObjectModel;

    public class Card : ScriptableObject
    {
        public enum RARITY
        {
            unset = -1,
            common = 0,
            uncommon = 1,
            rare = 2,
            legendary = 4,
            godlike = 8 
        };

        [SerializeField][ReadOnly]
        private float floatRarity;

        [SerializeField][ReadOnly]
        private int cost = -1;

        [SerializeField]
        private string description;

        private List<Effect> effects;

        private List<CardName> preNames;
        private CardName mainName;
        private List<CardName> postNames;

        private RARITY rarity = RARITY.unset;

        public RARITY Rarity {
            get {
                if(this.rarity == RARITY.unset)
                    this.rarity = (RARITY)(Mathf.RoundToInt(this.floatRarity * (int)RARITY.godlike));
                return this.rarity;
            } }

        public int Cost {
            get
            {
                if (this.cost <= 0)
                    this.cost = Mathf.CeilToInt(30f.Pow(this.floatRarity));
                return this.cost;
            }
        }

        public string Description { get { return this.description; } }

        public CardName MainName { get { return this.mainName; } }

        public Card()
        {
            this.effects = new List<Effect>();
            this.preNames = new List<CardName>();
            this.postNames = new List<CardName>();
        }

        #region Name String Creation
        public string UpdateName()
        {
            this.name = MergeNames(this.preNames) + this.mainName + " " + MergeNames(this.postNames);

            this.name = this.name.TrimEnd();
            return this.name;
        }

        /// <summary>
        /// Creates a single string from a list of names.
        /// </summary>
        /// <param name="namesList">The list of names to be used.</param>
        /// <returns>A (hopefully) comprehensible string containing all the names passed.</returns>
        private string MergeNames(List<CardName> namesList)
        {
            string merged = "";
            for (int index = 0; index < namesList.Count; index++)
            {
                if (index > 0)
                    merged += "and " + namesList[index] + " ";
                else if (namesList[0].PrepositionsCount > 0)
                {
                    merged = namesList[0].ToString(-1) + " ";
                } else 
                    merged += namesList[index] + " ";
            }

            return merged;
        }
        #endregion

        public void AppendName(CardName name)
        {
            AddName(name);
            this.floatRarity += name.Rarity;
            AddEffects(name.Effects);
        }

        private void AddName(CardName name)
        {
            switch (name.Type)
            {
                case CardName.NameType.PreName:
                    preNames.Add(name);
                    break;
                case CardName.NameType.MainName:
                    mainName = name;
                    break;
                case CardName.NameType.PostName:
                    postNames.Add(name);
                    break;
            }
        }

        public void RemoveName(CardName nameToRemove)
        {
            RemoveName(nameToRemove.name);
        }

        public void RemoveName(string nameStr)
        {
            bool nameRemoved = this.preNames.RemoveAll(cardName => cardName.name.Equals(nameStr)) > 0;
            nameRemoved |= this.postNames.RemoveAll(cardName => cardName.name.Equals(nameStr)) > 0;
            if(nameRemoved)
                RefreshEffects();
        }

        private void RefreshEffects()
        {
            this.effects.Clear();
            AddEffects(this.mainName.Effects);
            this.preNames.ForEach(cardName => AddEffects(cardName.Effects));
            this.postNames.ForEach(cardName => AddEffects(cardName.Effects));
        }

        public void AddEffect(Effect effectToAdd)
        {
            var modifier = effectToAdd as AttributeModifier;
            if (modifier)
            {
                if (this.effects.Find(effect =>
                 {
                     var modifierInList = effect as AttributeModifier;
                     if (modifierInList && modifierInList.Merge(modifier))
                         return true;
                     return false;
                 }))
                    return;

            }
            this.effects.Add(effectToAdd);
        }

        public void AddEffects(IEnumerable<AttributeModifier> effects)
        {
            foreach (Effect e in effects)
                AddEffect(e);
        }

        public ReadOnlyCollection<Effect> GetEffects() { return effects.AsReadOnly(); }

        public override string ToString()
        {
            return this.name;
        }

        public string ToObjectString()
        {
            string effectsStr = "";
            if (this.effects.Count > 0)
            {
                effectsStr += "[";
                this.effects.ForEach((effect) => { effectsStr += effect + "]; ["; });
                effectsStr = effectsStr.Substring(0, effectsStr.Length - 3);
            }
            return base.ToString() + " -> Cost: " + this.cost + ", Effects: {" + effectsStr + "}";
        }

        public override bool Equals(object o)
        {
            var otherCard = o as Card;
            if (!otherCard)
                return false;

            if (!otherCard.mainName.Equals(this.mainName))
                return false;

            if (otherCard.preNames.Count != this.preNames.Count || !otherCard.preNames.ContainsAll(this.preNames))
                return false;

            if (otherCard.postNames.Count != this.postNames.Count || !otherCard.postNames.ContainsAll(this.postNames))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = this.mainName.GetHashCode();
            this.preNames.ForEach(name => hashCode ^= name.GetHashCode());
            this.postNames.ForEach(name => hashCode ^= name.GetHashCode());
            return hashCode;
        }
    }
}