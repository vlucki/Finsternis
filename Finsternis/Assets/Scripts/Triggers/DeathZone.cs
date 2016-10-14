namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;

    public class DeathZone : Trigger
    {

        void Start()
        {
            if (!onExit)
                onExit = new OnTriggerEvent();

            layersToIgnore = 1 << LayerMask.NameToLayer("UI");
            onExit.AddListener(OnExit);
        }

        private void OnExit(GameObject go)
        {
            UnityQuery.Log.Info(this, "{0} exiting deathzone", go);
            GameManager.Instance.Kill(go);
        }
    }
}