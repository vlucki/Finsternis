namespace Finsternis {
    using UnityEngine;
    using System.Collections;
    using System;
    
    public class NotificationsController : MonoBehaviour {

        private Entity player;

        [SerializeField]
        private GameObject notificationPrefab;

        void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
            Inventory i = player.GetComponent<Inventory>();

            i.onCardAdded.AddListener(CardAdded);
            i.onCardRemoved.AddListener(CardRemoved);
        }

        private void CardRemoved(Card c)
        {
            GameObject notification = Instantiate(notificationPrefab);
        }

        private void CardAdded(Card c)
        {
            GameObject notification = Instantiate(notificationPrefab);
        }
    }
}