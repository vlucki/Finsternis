﻿namespace Finsternis
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
        private List<Card> cards;

        private Entity owner;

        private int totalInventoryCost;

        private int maximumCostAllowed = 10;

        public int MaximumCostAllowed
        {
            get { return this.maximumCostAllowed; }
            set { this.maximumCostAllowed = Mathf.Max(0, value); }
        }
        
        void Awake()
        {
            owner = GetComponent<Entity>();
        }

        //return true if the item was successfuly added to the inventory
        public bool AddCard(Card card)
        {
            bool cardAdded = false;
            if (totalInventoryCost <= maximumCostAllowed - card.Cost)
            {
                if (!cards.Contains(card))
                {
                    totalInventoryCost += card.Cost;
                    cardAdded = true;
                    cards.Add(card);

                    foreach(var e in card.GetEffects())
                    {
                        var e2 = e as AttributeModifier;
                        if (e2)
                        {
                            var attrib = owner.GetAttribute(e2.AttributeAlias);
                            if (attrib)
                            {
                                if (e2.ChangeType == AttributeModifier.ModifierType.Absolute)
                                    attrib.SetModifier(attrib.ValueModifier + e2.ValueChange);
                            }
                        }
                    }
                    
                    onCardAdded.Invoke(card);
                }
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
            totalInventoryCost -= c.Cost;
            onCardAdded.Invoke(c);
            cards.RemoveAt(cardIndex);
        }
    }
}
