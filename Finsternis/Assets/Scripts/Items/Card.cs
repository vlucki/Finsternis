using System.Collections.Generic;
using UnityEngine;

namespace Finsternis
{
    public class Card : ScriptableObject
    {
        public enum RARITY : byte
        {
            common      = 0, //0000
            uncommon    = 1, //0001
            rare        = 2, //0010
            legendary   = 4, //0100
            godlike     = 8  //1000
        };

        [SerializeField]
        private float rarity;

        [SerializeField]
        private int cost;

        [SerializeField]
        private string description;
        
        private List<Effect> effects;

        public RARITY Rarity { get { return (RARITY)this.rarity; } }

        public int Cost { get { return this.cost; } }

        public string Description { get { return this.description; } }

        public Card()
        {
            effects = new List<Effect>();
        }

        public void AppendName(CardName name)
        {
            this.name += name.name;
            this.rarity += name.Rarity;
            AddEffects(name.Effects);
        }
        
        public void AddEffect(Effect effect)
        {
            if (effect.InteractionType == Effect.EffectInteractionType.stackable)
            {
                this.effects.Add(effect);
            }
            else
            {
                for (int i = 0; i < this.effects.Count; i++)
                {
                    if (this.effects[i].GetType().Equals(effect.GetType()))
                    {
                        if(effect.InteractionType == Effect.EffectInteractionType.overwrite)
                        {
                            this.effects[i] = effect;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
        }

        public void AddEffects(IEnumerable<Effect> effects)
        {
            foreach (Effect e in effects)
                AddEffect(e);
        }

        public List<Effect> GetEffects() { return new List<Effect>(effects); }
    }
}