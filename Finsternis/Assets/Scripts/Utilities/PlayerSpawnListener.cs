namespace Finsternis
{
    using System;
    using UnityEngine;

    public class PlayerSpawnListener : CustomBehaviour
    {
        public UnityEngine.Events.UnityEvent onPlayerSpawned;

        protected void Awake()
        {
            GameManager.Instance.onPlayerSpawned += (PlayerSpawned);
        }

        private void PlayerSpawned(CharController player)
        {
            GameManager.Instance.onPlayerSpawned -= (PlayerSpawned);
            onPlayerSpawned.Invoke();
        }
    }
}