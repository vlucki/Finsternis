namespace Finsternis
{
    using UnityEngine;
    using System.Collections;

    public abstract class GlobalEventTrigger : MonoBehaviour
    {
        protected string eventName;

        public abstract void TriggerEvent();

    }
}