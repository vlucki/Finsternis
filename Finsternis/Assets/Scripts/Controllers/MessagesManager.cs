namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class MessagesManager : MonoBehaviour
    {

        [SerializeField]
        private GameObject messagePrefab;

        private List<MessageController> messagePool;
        private Dictionary<GameObject, MessageController> displayedMessages;

        private static MessagesManager activeManager;

        public static MessagesManager Instance
        {
            get
            {
                if (!activeManager)
                {
                    var messageManagerGO = GameObject.FindGameObjectWithTag("MessageManager");
                    if (messageManagerGO)
                        activeManager = messageManagerGO.GetComponent<MessagesManager>();
                }

                return activeManager;
            }
        }

        public MessageController ShowMessage(Vector3 position, string message, Sprite graphic = null, float duration = -1)
        {
            if (messagePool == null)
            {
                messagePool = new List<MessageController>();
            }
            var freeMessage = messagePool.Find((msg) => !msg.gameObject.activeSelf);
            if (!freeMessage)
            {
                freeMessage = Instantiate(messagePrefab).GetComponent<MessageController>();
                messagePool.Add(freeMessage);
            }

            freeMessage.transform.position = position;
            freeMessage.Show(message, graphic, duration);
            return freeMessage;
        }

    }
}