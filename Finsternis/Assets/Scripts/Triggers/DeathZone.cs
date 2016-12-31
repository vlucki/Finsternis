namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;

    public class DeathZone : Trigger
    {
        protected void Start()
        {
            if (!onExit)
                onExit = new OnTriggerEvent();

            layersToIgnore = 1 << LayerMask.NameToLayer("UI");
            onExit.AddListener(OnExit);
        }

        private void OnExit(GameObject go)
        {
#if LOG_INFO
            UnityQuery.Debug.LogFormat(this, "{0} exiting deathzone", go);
#endif
            GameManager.Instance.Kill(go);
            foreach (var renderer in go.GetComponentsInChildren<Renderer>())
                renderer.enabled = false;
        }
    }
}