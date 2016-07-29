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
        private RARITY rarity;

        [SerializeField]
        private int cost;

        [SerializeField]
        private string description;

        public RARITY Rarity { get { return this.rarity; } }

        public int Cost { get { return this.cost; } }

        public string Description { get { return this.description; } }

        public void Init(RARITY rarity, int cost, string description = "???")
        {
            this.rarity = rarity;
            this.cost = cost;
            this.description = description;
        }
    }
}