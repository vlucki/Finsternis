namespace Finsternis
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using EasyEditor;

    [RequireComponent(typeof(Entity))]
    public class Inventory : MonoBehaviour
    {
        [Serializable]
        public class InventoryEvent : Events.CustomEvent<Card> { }

        [Inspector(group = "Events", foldable =true, groupDescription = "Events called when something changes in the inventory")]
        public InventoryEvent onCardAdded;
        public InventoryEvent onCardRemoved;
        public InventoryEvent onCardEquipped;
        public InventoryEvent onCardUnequipped;

        [Inspector(group ="Cards")]
        [SerializeField]
        [ReadOnly]
        private List<CardStack> unequippedCards;

        [SerializeField]
        [ReadOnly]
        private List<CardStack> equippedCards;

        private HashSet<Card> newCards;

        private Entity owner;

        private int allowedCardPoints = 10;

        public int TotalEquippedCost { get; private set; }

        public int AllowedCardPoints
        {
            get { return this.allowedCardPoints; }
        }

        public void SetAllowedCardPoints(int value)
        {
            if (value >= 0)
                this.allowedCardPoints = value;
            while (this.allowedCardPoints < this.TotalEquippedCost && !this.equippedCards.IsNullOrEmpty())
            {
                UnequipCard(this.equippedCards[this.equippedCards.Count - 1].card);
            }
        }

        public void AddPoints(int v)
        {
            if (v > 0)
                this.allowedCardPoints += v;
        }

        public List<CardStack> EquippedCards { get { return this.equippedCards; } }
        public List<CardStack> UnequippedCards { get { return this.unequippedCards; } }
        
        public bool IsCardNew(Card card)
        {
            return newCards.Contains(card);
        }

        public void RemoveFromNew(Card card)
        {
            newCards.Remove(card);
        }

        void Awake()
        {
            this.owner = GetComponent<Entity>();
            this.unequippedCards = new List<CardStack>();
            this.equippedCards = new List<CardStack>();
            this.newCards = new HashSet<Card>();
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
            if (!CanEquip(card))
                return false;
            
            StackCard(this.equippedCards, card);

            this.TotalEquippedCost += card.Cost;
            UpdateCardEffects(card);

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

                this.TotalEquippedCost -= card.Cost;
                UpdateCardEffects(card, false);
                AddCard(card);

                onCardUnequipped.Invoke(card);

                return true;
            }
            return false;
        }

        private void UpdateCardEffects(Card card, bool addingNewEffects = true)
        {
            foreach (var effect in card.GetEffects())
            {
                var attributeModifier = effect as AttributeModifier;
                if (attributeModifier)
                {
                    var attrib = owner.GetAttribute(attributeModifier.AttributeAlias);
                    if (attrib)
                    {
                        if (addingNewEffects)
                            attrib.AddModifier(attributeModifier);
                        else
                            attrib.RemoveModifier(attributeModifier);
                    }
                }
            }
        }

        //return true if the item was successfuly added to the inventory
        public void AddCard(Card card)
        {
            if (!StackCard(this.unequippedCards, card))
                newCards.Add(card);
            onCardAdded.Invoke(card);
        }

        public void RemoveCard(Card card)
        {
            //If the card is not on the unequipped list
            if(!RemoveCardFromList(this.unequippedCards, card))
            {
                //try removing it from the equipped list
                if (!RemoveCardFromList(this.equippedCards, card))
                    return;

                onCardUnequipped.Invoke(card);
            }

            onCardRemoved.Invoke(card);
        }

        /// <summary>
        /// Tries to add a card to a pre-existing stack on a given list, creating a new stack if needed.
        /// </summary>
        /// <param name="list">List of card stacks.</param>
        /// <param name="card">Card do be added.</param>
        /// <returns>True if the card was stacked, false if a new stack was created.</returns>
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

        internal bool CanEquip(Card card)
        {
            return this.allowedCardPoints >= this.TotalEquippedCost + card.Cost;
        }
    }
}
