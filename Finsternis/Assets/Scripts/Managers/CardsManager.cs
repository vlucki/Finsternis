namespace Finsternis
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CardsManager : MonoBehaviour
    {
        public UnityEngine.Events.UnityEvent onCardGivenToPlayer;

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

        public void Start()
        {
            this.cardFactory = new CardFactory(this.cardNames);
        }

        public void GivePlayerCard(int quantity)
        {
            if (this.cardNames == null || this.cardNames.Count == 0)
            {
#if DEBUG
                Debug.LogErrorFormat(this, "No names attatched to manager. Aborting card generation.");
#endif
                return;
            }

            if(this.cardFactory == null)
            {
#if DEBUG
                Debug.LogErrorFormat(this, "No card factory found. Aborting card generation.");
#endif
                return;
            }
            while((--quantity) >= 0)
                PlayerInventory.AddCard(this.cardFactory.MakeCard());

            onCardGivenToPlayer.Invoke();
        }
    }
}
