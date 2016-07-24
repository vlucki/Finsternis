using UnityEngine;

namespace Finsternis
{
    public class Card : ScriptableObject
    {
        public enum RARITY
        {
            common = 0x0000,
            uncommon = 0x0001,
            rare = 0x0010,
            legendary = 0x0100,
            godlike = 0x1000
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