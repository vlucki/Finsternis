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
            onExit.AddListener(OnExit);
        }

        private void OnExit(GameObject go)
        {
            print("Exiting deathzone");
            GameManager.Instance.Kill(go);
        }
    }
}