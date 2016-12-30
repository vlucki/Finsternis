namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class MessagesManager : MonoBehaviour
    {

        [SerializeField]
        private GameObject staticMessagePrefab;

        [SerializeField]
        private GameObject dynamicMessagePrefab;

        private List<MessageController> staticMessagePool;
        private List<MessageController> dynamicMessagePool;


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

        public MessageController ShowStaticMessage(Vector3 position, string message, Sprite graphic = null, float duration = -1)
        {
            if (staticMessagePool == null)
            {
                staticMessagePool = new List<MessageController>();
            }

            return ShowMessage(this.staticMessagePool, this.staticMessagePrefab, position, message, graphic, duration);
        }

        public MessageController ShowDynamicMessage(Vector3 position, string message, Vector3 force = default(Vector3), Sprite graphic = null, float duration = -1)
        {
            if (dynamicMessagePool == null)
            {
                dynamicMessagePool = new List<MessageController>();
            }

            MessageController ctrl = ShowMessage(this.dynamicMessagePool, this.dynamicMessagePrefab, position, message, graphic, duration);

            if (force != Vector3.zero)
            {
                var body = ctrl.GetCachedComponent<Rigidbody>();
                if(!body)
                {
                    body = ctrl.GetComponent<Rigidbody>();
                    ctrl.CacheComponent(body);
                }
               body.AddForce(force, ForceMode.Impulse);
            }

            return ctrl;
        }

        private MessageController ShowMessage(List<MessageController> messagePool, GameObject prefab, Vector3 position, string message, Sprite graphic, float duration)
        {
            MessageController freeMessage = messagePool.Find(msg => !msg.gameObject.activeSelf);
            if (!freeMessage)
            {
                freeMessage = Instantiate(prefab).GetComponent<MessageController>();
                messagePool.Add(freeMessage);
            }

            freeMessage.transform.position = position;
            freeMessage.Show(message, graphic, duration);
            return freeMessage;
        }

    }
}