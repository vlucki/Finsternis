using UnityEngine;
using System.Collections.Specialized;
using System;

[Serializable]
public class Inventory : MonoBehaviour
{

    #region Slot
    [Serializable]
    //stores the itens and their quantity
    private class InventorySlot : ScriptableObject
    {
        [SerializeField]
        private Item _item;

        [SerializeField]
        private int _quantity;

        public Item Item
        {
            get { return _item; }
            set { if (!(_item = value)) _quantity = 0; }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if( (_quantity = value) <= 0)
                {
                    _quantity = 0;
                    _item = null;
                }
            }
        }

        public bool IsEmpty()
        {
            return _quantity == 0;
        }

        public InventorySlot() { }

        public InventorySlot(Item i, int quantity = 0)
        {
            this._item = i;
            this._quantity = quantity;
        }

    }
    #endregion

    //every inventory has a fixed number of item slots
    private InventorySlot[] _slots;

    public void Init(int size)
    {
        _slots = new InventorySlot[size];
        for(int i = 0; i < size; i++)
        {
            _slots[i] = ScriptableObject.CreateInstance<InventorySlot>();
        }
    }

    //return true if the item was successfuly added to the inventory
    public bool AddItem(Item item, int quantity = 1)
    {
        InventorySlot emptySlot = null;

        for(int index = 0; index < _slots.Length; index++){
            InventorySlot slot = _slots[index];
            if (slot.IsEmpty() && !emptySlot)
            {
                emptySlot = slot; //store the empty slot in case we need it
            }

            //if the slot being checked is holding the item that should be added, just increase it's quantity
            if (slot.Item.Equals(item))
            {
                slot.Quantity+= quantity;
                return true;
            }
        }

        //if the item passed is not yet in the inventory and there is an empty slot for it, store it
        if (emptySlot)
        {
            emptySlot.Item = item;
            emptySlot.Quantity = quantity;
            return true;
        }

        return false;
    }
}
