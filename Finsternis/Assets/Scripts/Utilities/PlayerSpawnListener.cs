namespace Finsternis
{
    using System;
    using UnityEngine;

    public class PlayerSpawnListener : CustomBehaviour
    {
        public UnityEngine.Events.UnityEvent onPlayerSpawned;

        protected void Awake()
        {
            GameManager.Instance.onPlayerSpawned.AddListener(PlayerSpawned);
        }

        private void PlayerSpawned(CharController player)
        {
            GameManager.Instance.onPlayerSpawned.RemoveListener(PlayerSpawned);
            onPlayerSpawned.Invoke();
        }
    }
}