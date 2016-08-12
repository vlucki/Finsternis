using System.Collections.Generic;
using UnityEngine;

namespace Finsternis
{
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

        private List<CardName> names;

        public RARITY Rarity { get { return (RARITY)this.rarity; } }

        public int Cost { get { return this.cost; } }

        public string Description { get { return this.description; } }

        public Card()
        {
            this.effects = new List<Effect>();
            this.names = new List<CardName>();
        }
        #region Name String Creation
        public string UpdateName()
        {
            this.name = "";
            int index = -1;
            names.ForEach(cardName =>
            {
                this.name += (GetAdditionalNameString(cardName, ++index) ?? "") + cardName.name + " ";
            });
            this.name = this.name.TrimEnd();
            return this.name;
        }

        private string GetAdditionalNameString(CardName cardName, int index)
        {
            if (index > 0 && cardName.Type != CardName.NameType.MainName)
            {
                if (names[index-1].Type == cardName.Type)
                    return "and ";
                else if (cardName.Type == CardName.NameType.PostName)
                {
                    return cardName.prepositions[Random.Range(0, cardName.prepositions.Count - 1)] + " ";
                }
            }
            return null;
        }
        #endregion

        public void AppendName(CardName name)
        {
            this.names.Add(name);
            this.rarity += name.Rarity;
            AddEffects(name.Effects);
        }

        public void RemoveName(CardName nameToRemove)
        {
            if (this.names.RemoveAll(cardName => cardName.Equals(nameToRemove)) > 0)
                RefreshEffects();
        }

        public void RemoveName(string nameStr)
        {
            if (this.names.RemoveAll(cardName => cardName.name.Equals(nameStr)) > 0)
                RefreshEffects();
        }

        private void RefreshEffects()
        {
            this.effects.Clear();
            this.names.ForEach(cardName => AddEffects(cardName.Effects));
        }

        private void RemoveName(System.Func<CardName, bool> condition)
        {
            effects.Clear(); //will have to recompute all effects since some may have been overwritte by the ones in the name that will be removed

            names.RemoveAll((cardName) =>
            {
                if (condition(cardName))
                    return true;

                AddEffects(cardName.Effects); //if this name won't be removed, re-add it's effects to the card
                return false;
            });
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
    }
}