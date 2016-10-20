namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;

    public class CardsManager : MonoBehaviour
    {
        [SerializeField]
        private CardGenerationParameters parameters;

        private Inventory playerInventory;
        private Inventory PlayerInventory
        {
            get
            {
                if (!this.playerInventory)
                    this.playerInventory = GameManager.Instance.Player.GetComponent<Inventory>();
                return this.playerInventory;
            }
        }

        public void GivePlayerCard(int quantity)
        {
            if (!this.parameters)
            {
                Log.Error(this, "No parameters attatched to manager. Aborting card generation.");
            }
            while((--quantity) >= 0)
                PlayerInventory.AddCard(CardFactory.MakeCard(this.parameters));
        }
    }
}
