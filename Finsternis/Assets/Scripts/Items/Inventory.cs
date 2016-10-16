namespace Finsternis
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [RequireComponent(typeof(Entity))]
    public class Inventory : MonoBehaviour
    {
        [Serializable]
        public class InventoryEvent : CustomEvent<Card> { }

        public InventoryEvent onCardAdded;
        public InventoryEvent onCardRemoved;
        public InventoryEvent onCardEquipped;
        public InventoryEvent onCardUnequipped;

        [SerializeField]
        [ReadOnly]
        private List<CardStack> unequippedCards;

        [SerializeField]
        [ReadOnly]
        private List<CardStack> equippedCards;

        private Entity owner;

        private int totalEquippedCost;

        private int maxEquippedCost = 10;

        public int MaximumCostAllowed
        {
            get { return this.maxEquippedCost; }
            set { this.maxEquippedCost = Mathf.Max(0, value); }
        }

        public List<CardStack> EquippedCards { get { return this.equippedCards; } }
        public List<CardStack> UnequippedCards { get { return this.unequippedCards; } }

        public List<Card> GetAllCards()
        {
            HashSet<Card> cards = new HashSet<Card>();
            this.equippedCards.ForEach(stack => cards.Add(stack.card));
            this.unequippedCards.ForEach(stack => cards.Add(stack.card));
            return cards.ToList();
        }

        void Awake()
        {
            this.owner = GetComponent<Entity>();
            this.unequippedCards = new List<CardStack>();
            this.equippedCards = new List<CardStack>();
        }

        public bool EquipCard(CardStack stack)
        {
            int index = this.unequippedCards.IndexOf(stack);
            if (index < 0 || !EquipCard(stack.card))
                return false;

            if (stack.RemoveCard())
                this.unequippedCards.RemoveAt(index);

            return true;
        }

        public bool EquipCard(Card card)
        {
            if (this.totalEquippedCost + card.Cost >= maxEquippedCost)
                return false;

            StackCard(this.equippedCards, card);

            this.totalEquippedCost += card.Cost;
            ApplyCardEffects(card);

            onCardEquipped.Invoke(card);

            return true;
        }

        public bool UnequipCard(Card card)
        {
            CardStack stack = GetStack(this.equippedCards, card);
            if (stack)
            {
                if (stack.RemoveCard())
                    equippedCards.Remove(stack);
                else
                    AddCard(card);

                onCardUnequipped.Invoke(card);
                return true;
            }
            return false;
        }

        private void ApplyCardEffects(Card card)
        {
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

        //return true if the item was successfuly added to the inventory
        public void AddCard(Card card)
        {
            StackCard(this.unequippedCards, card);
            onCardAdded.Invoke(card);
        }

        public void RemoveCard(Card card)
        {
            if(!RemoveCardFromList(this.unequippedCards, card))
            {
                if (!RemoveCardFromList(this.equippedCards, card))
                    return;
                onCardUnequipped.Invoke(card);
            }

            onCardRemoved.Invoke(card);
        }

        private bool StackCard(List<CardStack> list, Card card)
        {
            CardStack stack = GetStack(list, card);
            if (stack)
            {
                stack.AddCard();
                return true;
            }
            else
            {
                stack = new CardStack(card);
                list.Add(stack);
                return false;
            }
        }

        private bool RemoveCardFromList(List<CardStack> list, Card card)
        {
            CardStack stack = GetStack(list, card);
            if (stack)
            {
                if (stack.RemoveCard())
                    list.Remove(stack);

                return true;
            }
            return false;
        }

        public CardStack GetStack(List<CardStack> list, Card card)
        {
            return list.Find(stack => stack.card.Equals(card));
        }

        public bool IsEquipped(Card card)
        {
            return GetStack(this.equippedCards, card) != null;
        }
    }
}
