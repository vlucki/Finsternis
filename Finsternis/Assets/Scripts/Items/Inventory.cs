using UnityEngine;
using System.Collections.Specialized;
using System;

namespace Finsternis {
    [Serializable]
    public class Inventory : MonoBehaviour
    {

        //stores the itens and their quantity
        private class InventorySlot : ScriptableObject
        {
            [SerializeField]
            private Card card;

            [SerializeField]
            private int quantity;

            public Card Card
            {
                get { return this.card; }
                set { if (!(this.card = value)) this.quantity = 1; }
            }

            public int Quantity
            {
                get { return this.quantity; }
                set
                {
                    if ((this.quantity = value) <= 0)
                    {
                        this.quantity = 0;
                        this.card = null;
                    }
                }
            }

            public bool IsEmpty { get { return this.quantity == 0; } }

            public InventorySlot() { }

            public InventorySlot(Card i, int quantity = 1)
            {
                this.card = i;
                Quantity = quantity;
            }

        }

        //every inventory has a fixed number of item slots
        private InventorySlot[] slots;

        public void Init(int size)
        {
            this.slots = new InventorySlot[size];
            for (int i = 0; i < size; i++)
            {
                this.slots[i] = ScriptableObject.CreateInstance<InventorySlot>();
            }
        }

        //return true if the item was successfuly added to the inventory
        public bool AddItem(Card item, int quantity = 1)
        {
            InventorySlot emptySlot = null;

            for (int index = 0; index < slots.Length; index++)
            {
                InventorySlot slot = slots[index];
                if (slot.IsEmpty() && !emptySlot)
                {
                    emptySlot = slot; //store the empty slot in case we need it
                }

                //if the slot being checked is holding the item that should be added, just increase it's quantity
                if (slot.Card.Equals(item))
                {
                    slot.Quantity += quantity;
                    return true;
                }
            }

            //if the item passed is not yet in the inventory and there is an empty slot for it, store it
            if (emptySlot)
            {
                emptySlot.Card = item;
                emptySlot.Quantity = quantity;
                return true;
            }

            return false;
        }
    }
}
