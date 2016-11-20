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

        [SerializeField][ReadOnly]
        private GameObject[] visibleUnequippedCards;

        [SerializeField]
        [ReadOnly]
        private GameObject[] visibleEquippedCards;

        private int unequippedSelection = -1;
        private int equippedSelection = -1;

        private Inventory inventory;
        private InputRouter input;

        protected override void Awake()
        {
            base.Awake();

            this.input = GetComponent<InputRouter>();

            Transform unequippedPanel = transform.Find("UnequippedPanel");
            visibleUnequippedCards = GetCards(unequippedPanel);

            Transform equippedPanel = transform.Find("EquippedPanel");
            visibleEquippedCards = GetCards(equippedPanel);

            if (!GetInventory())

#if DEBUG
                Log.E(this, "Could not find player inventory!");
#endif
        }

        private GameObject[] GetCards(Transform cardsPanel)
        {
            return new GameObject[]
            {
                cardsPanel.FindDescendant("TopCard").gameObject,
                cardsPanel.FindDescendant("SelectedCard").gameObject,
                cardsPanel.FindDescendant("BottomCard").gameObject

            };
        }

        private Inventory GetInventory()
        {
            if (!this.inventory)
                this.inventory = GameManager.Instance.Player.GetComponent<Inventory>();

            if (this.inventory)
            {
                this.inventory.onCardAdded.AddListener(card => UpdateUnequippedDisplay());
                this.inventory.onCardRemoved.AddListener(card => UpdateUnequippedDisplay());
                this.inventory.onCardEquipped.AddListener(card => UpdateEquippedDisplay());
                this.inventory.onCardUnequipped.AddListener(card => UpdateEquippedDisplay());
            }
            return this.inventory;
        }

        public override void BeginOpening()
        {
            base.BeginOpening();

            UpdateUnequippedDisplay();
            
            UpdateEquippedDisplay();
        }
        
        #region display methods
        private bool ActivateCards(GameObject[] display, IList cardsList, ref int selection)
        {
            foreach (var visibleCard in display)
                visibleCard.SetActive(false);
            
            if (cardsList.Count == 0)
                return false;

            selection = Mathf.Max(selection, 0);
            return true;
        }

        int UpdateDisplay(List<CardStack> cards, GameObject[] visibleCards, int selection)
        {
            if (ActivateCards(visibleCards, cards, ref selection))
            {
                ShowCardDisplay(1, selection, visibleCards, cards);

                if (cards.Count > 1)
                {
                    if (cards.Count == 2) //if only 2 cards are equipped, either the display above or below won't be visible
                    {
                        //if currently selected = 0, display the card "below" (visibleCards[2]) -> 2 - 0 * 2 = 2
                        //if currently selected = 1, display the card "above" (visibleCards[0]) -> 2 - 1 * 2 = 0
                        int visibleCardIndex = 2 - selection * 2;
                        ShowCardDisplay(visibleCardIndex, selection, visibleCards, cards);
                    }
                    else
                    {
                        ShowCardDisplay(0, selection, visibleCards, cards);
                        ShowCardDisplay(2, selection, visibleCards, cards);
                    }
                }
            }
            return selection;
        }

        void UpdateUnequippedDisplay()
        {
            var unequipped = this.inventory.UnequippedCards;
            this.unequippedSelection = UpdateDisplay(unequipped, this.visibleUnequippedCards, this.unequippedSelection);
            //if (!ActivateCards(this.visibleUnequippedCards, unequipped, ref this.unequippedSelection))
            //    return;

            //ShowCardDisplay(1, this.unequippedSelection, this.visibleUnequippedCards, unequipped);

            //if (unequipped.Count > 1)
            //{
            //    if (unequipped.Count == 2) //if only 2 cards are not equipped, either the display above or below won't be visible
            //    {
            //        //if currently selected = 0, display the card "below" (visibleCards[2]) -> 2 - 0 * 2 = 2
            //        //if currently selected = 1, display the card "above" (visibleCards[0]) -> 2 - 1 * 2 = 0
            //        int visibleCardIndex = 2 - this.unequippedSelection * 2;
            //        ShowCardDisplay(visibleCardIndex, this.unequippedSelection, this.visibleUnequippedCards, unequipped);
            //    }
            //    else
            //    {
            //        ShowCardDisplay(0, this.unequippedSelection, this.visibleUnequippedCards, unequipped);
            //        ShowCardDisplay(2, this.unequippedSelection, this.visibleUnequippedCards, unequipped);
            //    }
            //}
        }

        void UpdateEquippedDisplay()
        {
            var equipped = this.inventory.EquippedCards; 
            if(!ActivateCards(this.visibleEquippedCards, equipped, ref this.equippedSelection))
                return;

            ShowCardDisplay(1, this.equippedSelection, this.visibleEquippedCards, equipped);

            if (equipped.Count > 1)
            {
                if (equipped.Count == 2) //if only 2 cards are equipped, either the display above or below won't be visible
                {
                    //if currently selected = 0, display the card "below" (visibleCards[2]) -> 2 - 0 * 2 = 2
                    //if currently selected = 1, display the card "above" (visibleCards[0]) -> 2 - 1 * 2 = 0
                    int visibleCardIndex = 2 - this.equippedSelection * 2;
                    ShowCardDisplay(visibleCardIndex, this.equippedSelection, this.visibleEquippedCards, equipped);
                }
                else
                {
                    ShowCardDisplay(0, this.equippedSelection, this.visibleEquippedCards, equipped);
                    ShowCardDisplay(2, this.equippedSelection, this.visibleEquippedCards, equipped);
                }
            }
        }

        void ShowCardDisplay(int displayIndex, int selectedIndex, GameObject[] displayArray, List<CardStack> cardsList)
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
            var controller = displayArray[displayIndex].GetComponent<CardDisplay>();
            controller.LoadStack(cardsList[listIndex]);
            if (displayIndex == 1)
                this.inventory.RemoveFromNew(controller.Card);
        }
        #endregion

        #region selection methods
        public void MoveUnequipped(float value)
        {
            if (value == 0)
                return;

            this.unequippedSelection = MoveSelection(this.unequippedSelection, this.inventory.UnequippedCards, value < 0 ? -1 : 1);
            UpdateUnequippedDisplay();
        }

        public void MoveEquipped(float value)
        {
            if (value == 0)
                return;

            this.equippedSelection = MoveSelection(this.equippedSelection, this.inventory.EquippedCards, value < 0 ? -1 : 1);
            UpdateEquippedDisplay();
        }

        private int MoveSelection(int currentlySelected, IList list, int displayOffset)
        {
            currentlySelected += displayOffset;
            if (currentlySelected < 0)
                currentlySelected = list.Count - 1;
            else if (currentlySelected >= list.Count)
                currentlySelected = 0;

            return currentlySelected;
        }

        public void EquipSelected(bool askForConfirmation)
        {
            var unequipped = this.inventory.UnequippedCards;

            if (unequipped.Count == 0)
                return;

            if (askForConfirmation)
            {
                this.input.Disable();
                confirmationDialog.Show(
                    "Equipped cards remain so until the end of the floor.", 
                    ()=> { this.input.Enable(); EquipSelected(false); },
                    ()=> { this.input.Enable(); }, 
                    "Equip!", 
                    "Cancel");
            }
            else
            {
                var equippedCard = unequipped[this.unequippedSelection].card;
                if (inventory.EquipCard(unequipped[this.unequippedSelection]))
                {
                    UpdateUnequippedSelection(equippedCard);
                    UpdateEquippedSelection(equippedCard);
                }
            }
        }

        private void UpdateEquippedSelection(Card equippedCard)
        { 
            //if the newly equipped card wasn't already selected, select it
            var equippedCards = this.inventory.EquippedCards;

            if (this.equippedSelection >= equippedCards.Count ||
                equippedCard != equippedCards[this.equippedSelection].card)
            {
                this.equippedSelection = this.inventory.EquippedCards.IndexOf(
                           this.inventory.GetStack(this.inventory.EquippedCards, equippedCard));
                UpdateEquippedDisplay();
            }
        }

        private void UpdateUnequippedSelection(Card equippedCard)
        {
            var unequippedCards = this.inventory.UnequippedCards;
            this.unequippedSelection = Mathf.Clamp(this.unequippedSelection, 0, unequippedCards.Count - 1);
            //If the equipped card was removed from the unequipped list, update the unequipped selection
            if (this.unequippedSelection >= unequippedCards.Count || this.unequippedSelection < 0 ||
                unequippedCards[this.unequippedSelection].card != equippedCard)
            {
                UpdateUnequippedDisplay();
            }
        }
        #endregion
    }
}