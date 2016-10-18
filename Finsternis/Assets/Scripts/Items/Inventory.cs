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

        private HashSet<Card> newCards;

        private Entity owner;

        private int totalEquippedCost;

        private int maximumCostAllowed = 10;

        public int MaximumCostAllowed
        {
            get { return this.maximumCostAllowed; }
            set { this.maximumCostAllowed = Mathf.Max(0, value); }
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
            if (this.totalEquippedCost + card.Cost >= maximumCostAllowed)
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
    }
}
