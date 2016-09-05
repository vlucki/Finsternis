namespace Finsternis
{
    using UnityEngine;
    using System.Collections;

    public class CardsManager : MonoBehaviour
    {

        Entity player;
        void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
        }

        public void GivePlayerCard(int quantity)
        {
            while(quantity-- > 0)
                player.GetComponent<Inventory>().AddCard(CardFactory.MakeCard());
        }
    }
}
