namespace Finsternis {
    using UnityEngine;
    using System.Collections;
    using System;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using UnityQuery;

    public class NotificationsController : MonoBehaviour {
        
        [SerializeField]
        private GameObject notificationPrefab;
        [SerializeField]
        private Canvas notificationCanvas;

        private Queue<GameObject> notificationsQueue;

        void Awake()
        {
            GameManager.Instance.OnPlayerSpawned.AddListener(Init);
        }

        void Init()
        {
            var player = GameManager.Instance.Player;
            if (!notificationCanvas)
                notificationCanvas = FindObjectOfType<Canvas>();
            Inventory i = player.GetComponent<Inventory>();
            notificationsQueue = new Queue<GameObject>();

            i.onCardAdded.AddListener(CardAdded);
            i.onCardRemoved.AddListener(CardRemoved);
        }

        private void CardRemoved(Card c)
        {
            PushNotification(c.name + " lost!");
        }

        private void CardAdded(Card c)
        {
            PushNotification(c.name + " found!");
        }

        private void PushNotification(string message)
        {
            GameObject notification = Instantiate(notificationPrefab);
            var transform = notification.GetComponent<RectTransform>();
            var defaultTransform = notificationPrefab.GetComponent<RectTransform>();

            transform.SetParent(notificationCanvas.transform);
            transform.localScale = defaultTransform.localScale;
            transform.anchoredPosition = defaultTransform.anchoredPosition;
            transform.GetChild(1).GetComponent<Text>().text = message;

            notification.GetComponent<FadeOutTransition>().OnTransitionEnded.AddListener(Dequeue);
            this.notificationsQueue.Enqueue(notification);
            if (this.notificationsQueue.Count == 1)
                notification.SetActive(true);
        }

        private void Dequeue(Transition t)
        {
            notificationsQueue.Dequeue().DestroyNow();
            if (notificationsQueue.Count > 0)
                notificationsQueue.Peek().SetActive(true);
        }
    }
}