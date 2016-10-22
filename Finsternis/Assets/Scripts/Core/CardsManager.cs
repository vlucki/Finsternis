namespace Finsternis
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityQuery;

    public class CardsManager : MonoBehaviour
    {
        [SerializeField]
        private CardGenerationParameters parameters;

        [SerializeField]
        private List<CardName> cardNames;

        private Inventory playerInventory;

        private CardFactory cardFactory;

        private Inventory PlayerInventory
        {
            get
            {
                if (!this.playerInventory)
                    this.playerInventory = GameManager.Instance.Player.GetComponent<Inventory>();
                return this.playerInventory;
            }
        }

        void Awake()
        {
            this.cardFactory = new CardFactory(this.cardNames);
        }

        public void GivePlayerCard(int quantity)
        {
            if (!this.parameters)
            {
                Log.Error(this, "No parameters attatched to manager. Aborting card generation.");
            }
            while((--quantity) >= 0)
                PlayerInventory.AddCard(this.cardFactory.MakeCard());
        }
    }
}
