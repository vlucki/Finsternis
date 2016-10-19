namespace Finsternis
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Card : ScriptableObject
    {
        public enum RARITY : byte
        {
            common = 0, //0000
            uncommon = 1, //0001
            rare = 2, //0010
            legendary = 4, //0100
            godlike = 8  //1000
        };

        [SerializeField]
        private float rarity;

        [SerializeField]
        private int cost;

        [SerializeField]
        private string description;

        private List<Effect> effects;

        private List<CardName> preNames;
        private CardName mainName;
        private List<CardName> postNames;

        public RARITY Rarity { get { return (RARITY)this.rarity; } }

        public int Cost { get { return this.cost; } }

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
            this.name = "";

            this.name += MergeNames(this.preNames) + this.mainName.name + MergeNames(this.postNames);

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
                merged += GetAdditionalNameString(namesList, index);
                merged += namesList[index].name + " ";
            }
            return merged;
        }

        private string GetAdditionalNameString(List<CardName> names, int index)
        {
            CardName cardName = names[index];
            if (index > 0 && cardName.Type != CardName.NameType.MainName)
            {
                if (names[index - 1].Type == cardName.Type)
                    return "and ";
                else if (cardName.Type == CardName.NameType.PostName)
                {
                    return cardName.prepositions[Random.Range(0, cardName.prepositions.Count - 1)] + " ";
                }
            }
            return "";
        }
        #endregion

        public void AppendName(CardName name)
        {
            AddName(name);
            this.rarity += name.Rarity;
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
                this.effects.Add((Effect)effectToAdd.Clone());
            }
        }

        public void AddEffects(IEnumerable<Effect> effects)
        {
            foreach (Effect e in effects)
                AddEffect(e);
        }

        public List<Effect> GetEffects() { return new List<Effect>(effects); }

        public override string ToString()
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

            return true;
        }

        public override int GetHashCode()
        {
            return this.mainName.GetHashCode();
        }
    }
}