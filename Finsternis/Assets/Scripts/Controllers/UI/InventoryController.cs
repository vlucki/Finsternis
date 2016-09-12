namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using System.Collections.Generic;
    using UnityQuery;
    using System.Linq;

    public class InventoryController : MenuController
    {
        [SerializeField]
        private ConfirmationDialogController confirmationDialog;
        [SerializeField]
        private GameObject[] visibleUnequippedCards;
        [SerializeField]
        private GameObject[] visibleEquippedCards;

        private int unequippedSelection = -1;
        private int equipmentSelection = -1;

        private Inventory inventory;

        private List<Card> unequipped;

        private Inventory GetInventory()
        {
            if (!this.inventory)
            {
                this.inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
                this.unequipped = new List<Card>(this.inventory.Cards.SkipWhile(this.inventory.EquippedCards.Contains));
                this.inventory.onCardAdded.AddListener(this.unequipped.Add);
                this.inventory.onCardRemoved.AddListener((card) => { this.unequipped.Remove(card); });
            }
            return this.inventory;
        }

        public override void BeginOpening()
        {
            base.BeginOpening();
            UpdateUnequippedDisplay();
            UpdateEquippedDisplay();
        }

        void UpdateUnequippedDisplay()
        {
            if(!GetInventory())
            {
                this.Error("Could not find player inventory!");
                return;
            }
            foreach (var visibleCard in this.visibleUnequippedCards)
                visibleCard.SetActive(false);

            if (this.unequipped.Count == 0)
                return;

            if (this.unequippedSelection < 0)
                this.unequippedSelection = 0;

            ShowCardDisplay(1, this.unequippedSelection, this.visibleUnequippedCards, this.unequipped);

            if (this.unequipped.Count > 1)
            {
                if (this.unequipped.Count == 2) //if only 2 cards are not equipped, either the display above or below won't be visible
                {
                    //if currently selected = 0, display the card "below" (visibleCards[2]) -> 2 - 0 * 2 = 2
                    //if currently selected = 1, display the card "above" (visibleCards[0]) -> 2 - 1 * 2 = 0
                    int visibleCardIndex = 2 - this.unequippedSelection * 2;
                    ShowCardDisplay(visibleCardIndex, this.unequippedSelection, this.visibleUnequippedCards, this.unequipped);
                }
                else
                {
                    ShowCardDisplay(0, this.unequippedSelection, this.visibleUnequippedCards, this.unequipped);
                    ShowCardDisplay(2, this.unequippedSelection, this.visibleUnequippedCards, this.unequipped);
                }
            }
        }

        void UpdateEquippedDisplay()
        {
            if (!GetInventory())
            {
                this.Error("Could not find player inventory!");
                return;
            }

            foreach (var visibleCard in this.visibleEquippedCards)
                visibleCard.SetActive(false);

            var equipped = this.inventory.EquippedCards;
            if (equipped.Count == 0)
                return;

            if (this.equipmentSelection < 0)
                this.equipmentSelection = 0;

            ShowCardDisplay(1, this.equipmentSelection, this.visibleEquippedCards, equipped);

            if (equipped.Count > 1)
            {
                if (equipped.Count == 2) //if only 2 cards are equipped, either the display above or below won't be visible
                {
                    //if currently selected = 0, display the card "below" (visibleCards[2]) -> 2 - 0 * 2 = 2
                    //if currently selected = 1, display the card "above" (visibleCards[0]) -> 2 - 1 * 2 = 0
                    int visibleCardIndex = 2 - this.equipmentSelection * 2;
                    ShowCardDisplay(visibleCardIndex, this.equipmentSelection, this.visibleEquippedCards, equipped);
                }
                else
                {
                    ShowCardDisplay(0, this.equipmentSelection, this.visibleEquippedCards, equipped);
                    ShowCardDisplay(2, this.equipmentSelection, this.visibleEquippedCards, equipped);
                }
            }
        }

        void ShowCardDisplay(int displayIndex, int selectedIndex, GameObject[] displayArray, List<Card> cardsList)
        {
            //if display = 0, index in list = selected - 1
            //if display = 1, index in list = selected
            //if display = 2, index in list = selected + 1
            int listIndex = selectedIndex + displayIndex - 1;

            //Clamp index within list range, making it wrap around
            if (listIndex < 0)
                listIndex = cardsList.Count - 1;
            else if (listIndex >= cardsList.Count)
                listIndex = 0;

            displayArray[displayIndex].SetActive(true);
            displayArray[displayIndex].GetComponent<CardController>().LoadCard(cardsList[listIndex]);
        }

        public void MoveAlbum(float value)
        {
            if (value == 0)
                return;
            UpdateUnequippedSelection(value < 0 ? -1 : 1);
        }

        public void MoveEquipped(float value)
        {
            if (value == 0)
                return;
            UpdateEquipmentSelection(value < 0 ? -1 : 1);

        }

        public void EquipSelected(bool askForConfirmation)
        {
            if (askForConfirmation)
            {
                var input = GetComponent<InputRouter>();
                input.Disable();
                confirmationDialog.Show(
                    "Equipped cards remain so until the end of the floor.", 
                    ()=> { input.Enable(); EquipSelected(false); },
                    ()=> { input.Enable(); }, 
                    "Equip!", 
                    "Cancel");
            }
            else
            {
                if (inventory.EquipCard(this.unequipped[this.unequippedSelection]))
                {
                    this.unequipped.RemoveAt(this.unequippedSelection);
                    UpdateUnequippedSelection();
                    UpdateEquipmentSelection(this.inventory.EquippedCards.Count - this.equipmentSelection - 1); //select the newly equiped card
                }
            }
        }


        private void UpdateUnequippedSelection(int v = 0)
        {
            this.unequippedSelection += v;
            if (this.unequippedSelection < 0)
                this.unequippedSelection = this.unequipped.Count - 1;
            else if (this.unequippedSelection >= this.unequipped.Count)
                this.unequippedSelection = 0;

            UpdateUnequippedDisplay();
        }

        private void UpdateEquipmentSelection(int v = 0)
        {
            this.equipmentSelection += v;
            if (this.equipmentSelection < 0)
                this.equipmentSelection = this.inventory.EquippedCards.Count - 1;
            else if (this.equipmentSelection >= this.inventory.EquippedCards.Count)
                this.equipmentSelection = 0;

            UpdateEquippedDisplay();
        }
    }
}