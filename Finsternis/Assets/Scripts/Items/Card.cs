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

        public void AppendName(CardName name)
        {
            this.name += name.name;
            this.rarity += name.Rarity;
            this.effects.AddRange(name.Effects);
        }
    }
}