namespace Finsternis
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityQuery;
    using System.Linq;

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
        private int cost;

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

        public int Cost { get { return this.cost; } }

        public string Description { get { return this.description; } }

        public CardName MainName { get { return this.mainName; } }

        public Card()
        {
            this.cost = Random.Range(1, 3); //set base cost
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
            this.cost += Mathf.CeilToInt(name.Rarity * 10);
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
            Effect match = this.effects.Find(effect => effect.Merge(effectToAdd));
            if (!match)
            {
                this.effects.Add(effectToAdd);
            }
        }

        public void AddEffects(IEnumerable<AttributeModifier> effects)
        {
            foreach (Effect e in effects)
                AddEffect(e);
        }

        public List<Effect> GetEffects() { return new List<Effect>(effects); }

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