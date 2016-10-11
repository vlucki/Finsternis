namespace Finsternis
{
    using UnityEngine;

    public class CardsManager : MonoBehaviour
    {

        Inventory playerInventory;
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
            while((--quantity) >= 0)
                PlayerInventory.AddCard(CardFactory.MakeCard());
        }
    }
}
