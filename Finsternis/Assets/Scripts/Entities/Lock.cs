namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    [RequireComponent(typeof(OpeneableEntity))]
    public class Lock : MonoBehaviour
    {
        #region Custom events declaration
        [System.Serializable]
        public class UnlockableEvent : CustomEvent<Lock> { }
        #endregion

        #region Events
        public UnlockableEvent OnUnlock;
        #endregion

        [SerializeField]
        private bool permanentLock = false;

        private OpeneableEntity lockedEntity;

        void Awake()
        {
            this.lockedEntity = GetComponent<OpeneableEntity>();
            this.lockedEntity.EntityEvents.onInteraction.AddListener(TryUnlocking);
        }

        private KeyCard key;

        public void TryUnlocking()
        {
            if (Unlock())
            {
                OnUnlock.Invoke(this);
            }
        }

       protected virtual bool Unlock()
        {
            if (permanentLock)
                return false;

            var action = lockedEntity.LastInteraction as OpenAction;
            if (!action || !action.KeyCard || action.KeyCard.UsedUp)
                return false;

            bool correctKey = action.KeyCard.Equals(this.key);

            if (correctKey)
                action.KeyCard.Use();

            return correctKey;
        }

        public void SetKey(KeyCard c)
        {
            if (!c || key)
                return;
            this.key = c;
        }

        public void RemovePermaLock()
        {
            this.permanentLock = false;
        }

    }
}