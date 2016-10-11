namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using System.Collections.Generic;
    using UnityQuery;
    using System.Linq;

    public class InventoryMenuController : MenuController
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
                this.inventory = GameManager.Instance.Player.GetComponent<Inventory>();
                this.unequipped = new List<Card>(this.inventory.Cards.SkipWhile(this.inventory.EquippedCards.Contains));
                this.inventory.onCardAdded.AddListener(this.unequipped.Add);
                this.inventory.onCardRemoved.AddListener((card) => { this.unequipped.Remove(card); });                
            }
            return this.inventory;
        }

        protected override void Awake()
        {
            base.Awake();
            if (!GetInventory())
            {
                Log.Error(this, "Could not find player inventory!");
            }
        }

        public override void BeginOpening()
        {
            base.BeginOpening();

            UpdateUnequippedDisplay();
            
            UpdateEquippedDisplay();
        }

        void UpdateUnequippedDisplay()
        {
            if (!ActivateCards(this.visibleUnequippedCards, this.unequipped, ref this.unequippedSelection))
                return;

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

        private bool ActivateCards(GameObject[] display, List<Card> cardsList, ref int selection)
        {
            foreach (var visibleCard in display)
                visibleCard.SetActive(false);
            
            if (cardsList.Count == 0)
                return false;

            selection = Mathf.Max(selection, 0);
            return true;
        }

        void UpdateEquippedDisplay()
        {
            var equipped = this.inventory.EquippedCards; 
            if(!ActivateCards(this.visibleEquippedCards, equipped, ref this.equipmentSelection))
                return;

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
            if (this.unequipped.Count == 0)
                return;

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
            this.unequippedSelection = UpdateSelection(this.unequippedSelection, this.unequipped, v);

            UpdateUnequippedDisplay();
        }

        private void UpdateEquipmentSelection(int v = 0)
        {
            this.equipmentSelection = UpdateSelection(this.equipmentSelection, this.inventory.EquippedCards, v);
            UpdateEquippedDisplay();
        }

        private int UpdateSelection(int currentlySelected, IList list, int amount)
        {
            currentlySelected += amount;
            if (currentlySelected < 0)
                currentlySelected = list.Count - 1;
            else if (currentlySelected >= list.Count)
                currentlySelected = 0;

            return currentlySelected;
        }
    }
}