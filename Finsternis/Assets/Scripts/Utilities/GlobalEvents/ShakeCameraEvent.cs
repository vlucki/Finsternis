namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;

    public class ShakeCameraEvent : GlobalEventTrigger
    {

        public override void TriggerEvent()
        {
            if (GameManager.Instance)
            {
                GameManager.Instance.TriggerGlobalEvent("ShakeCamera", transform.position);
            }
        }
    }
}