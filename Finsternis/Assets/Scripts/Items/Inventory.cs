namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Specialized;
    using System;
    using System.Collections.Generic;

    [RequireComponent(typeof(Entity))]
    public class Inventory : MonoBehaviour
    {
        [Serializable]
        public class InventoryEvent : UnityEngine.Events.UnityEvent<Card> { }

        public InventoryEvent onCardAdded;
        public InventoryEvent onCardRemoved;

        [SerializeField]
        [ReadOnly]
        private List<Card> cards;

        [SerializeField]
        [ReadOnly]
        private List<Card> equippedCards;

        private Entity owner;

        private int totalEquippedCost;

        private int maxEquippedCost = 10;

        public int MaximumCostAllowed
        {
            get { return this.maxEquippedCost; }
            set { this.maxEquippedCost = Mathf.Max(0, value); }
        }

        public List<Card> EquippedCards { get { return this.equippedCards; } }
        public List<Card> Cards { get { return this.cards; } }

        void Awake()
        {
            owner = GetComponent<Entity>();
        }

        public bool EquipCard(Card card)
        {
            bool cardEquipped = false;
            if (totalEquippedCost <= maxEquippedCost - card.Cost)
            {
                cardEquipped = true;
                totalEquippedCost += card.Cost;

                foreach (var effect in card.GetEffects())
                {
                    var attributeModifier = effect as AttributeModifier;
                    if (attributeModifier)
                    {
                        var attrib = owner.GetAttribute(attributeModifier.AttributeAlias);
                        if (attrib)
                        {
                            attrib.AddModifier(attributeModifier);
                        }
                    }
                }
            }

            return cardEquipped;
        }

        //return true if the item was successfuly added to the inventory
        public bool AddCard(Card card)
        {
            bool cardAdded = false;
            if (!this.cards.Contains(card))
            {
                this.cards.Add(card);
                cardAdded = true;
                onCardAdded.Invoke(card);
            }

            return cardAdded;
        }

        public void RemoveCard(Card card)
        {
            RemoveCard(cards.IndexOf(card));
        }

        public void RemoveCard(int cardIndex)
        {
            Card c = cards[cardIndex];
            cards.RemoveAt(cardIndex);
            if (equippedCards.Remove(c))
            {
                totalEquippedCost -= c.Cost;
                foreach(var effect in c.GetEffects())
                {
                    var attributeModifier = effect as AttributeModifier;
                    if (attributeModifier)
                    {
                        var attrib = owner.GetAttribute(attributeModifier.AttributeAlias);
                        if (attrib)
                        {
                            attrib.RemoveModifier(attributeModifier);
                        }
                    }
                }
            }
            onCardRemoved.Invoke(c);
        }
    }
}
