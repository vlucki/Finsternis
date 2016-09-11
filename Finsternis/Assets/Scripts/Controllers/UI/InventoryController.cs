namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using System.Collections.Generic;
    using UnityQuery;

    public class InventoryController : MenuController
    {
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private ConfirmationDialogController confirmationDialog;
        private GameObject inventoryPanel;
        private GameObject equippedPanel;
        private GameObject[] visibleAlbumCards;
        private GameObject[] visibleEquippedCards;

        private int albumSelection = -1;
        private int equipmentSelection = -1;

        private Inventory inventory;

        protected override void Awake()
        {
            base.Awake();

            this.inventoryPanel = transform.Find("InventoryPanel").gameObject;
            this.visibleAlbumCards = new GameObject[]{
                (GameObject)Instantiate(cardPrefab, inventoryPanel.transform),
                (GameObject)Instantiate(cardPrefab, inventoryPanel.transform),
                (GameObject)Instantiate(cardPrefab, inventoryPanel.transform) };
            this.visibleAlbumCards[0].transform.localScale = this.visibleAlbumCards[2].transform.localScale = Vector3.one / 2;
            RectTransform cardTransform = this.visibleAlbumCards[0].GetComponent<RectTransform>();
        }

        private Inventory GetInventory()
        {
            if(!this.inventory)
                this.inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
            return this.inventory;
        }

        public override void BeginOpening()
        {
            base.BeginOpening();
            PopulateAlbum();
        }

        void PopulateAlbum()
        {
            if(!GetInventory())
            {
                this.Error("Could not find player inventory!");
                return;
            }
            foreach (var visibleCard in visibleAlbumCards)
                visibleCard.SetActive(false);

            if (inventory.Cards.Count == 0)
                return;

            if (albumSelection < 0)
                albumSelection = 0;

            ShowCardDisplay(1, albumSelection, visibleAlbumCards, inventory.Cards);

            if (inventory.Cards.Count > 1)
            {
                if (inventory.Cards.Count == 2) //if only 2 cards are in inventory, either the display above or below won't be visible
                {
                    //if currently selected = 0, display the card "below" (visibleCards[2]) -> 2 - 0 * 2 = 2
                    //if currently selected = 1, display the card "above" (visibleCards[0]) -> 2 - 1 * 2 = 0
                    int visibleCardIndex = 2 - albumSelection * 2;
                    ShowCardDisplay(visibleCardIndex, albumSelection, visibleAlbumCards, inventory.Cards);
                }
                else
                {
                    ShowCardDisplay(0, albumSelection, visibleAlbumCards, inventory.Cards);
                    ShowCardDisplay(2, albumSelection, visibleAlbumCards, inventory.Cards);
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
            UpdateAlbumSelection(value < 0 ? -1 : 1);
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
                confirmationDialog.Show<bool>("Equipped cards remain so until the end of the floor.", EquipSelected, false, null, "Equip!", "Cancel");
            }
            else
            {
                inventory.EquipCard(visibleAlbumCards[1].GetComponent<CardController>().Card);
            }
        }


        private void UpdateAlbumSelection(int v)
        {
            throw new NotImplementedException();
        }

        private void UpdateEquipmentSelection(int v)
        {
            throw new NotImplementedException();
        }
    }
}