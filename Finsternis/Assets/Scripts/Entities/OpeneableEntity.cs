namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System;

    public class OpeneableEntity : Entity, IOpeneable
    {

        [System.Serializable]
        public class OpeneableEvent : CustomEvent<OpeneableEntity> { }

        [Header("Open/Close events")]
        public OpeneableEvent OnOpen;
        public OpeneableEvent OnClose;
        public OpeneableEvent OnLockRemoved;

        [SerializeField]
        private List<EntityAction.ActionType> allowedActions;

        private bool isOpen;

        private List<Lock> locks;

        public bool IsLocked { get { return this.locks.Count > 0; } }

        public bool IsOpen { get { return this.isOpen; } }

        protected virtual void Awake()
        {
            locks = new List<Lock>();
            GetComponents<Lock>(locks);
            foreach (Lock l in locks)
            {
                l.OnUnlock.AddListener(RemoveLock);
            }
        }

        private void RemoveLock(Lock removedLock)
        {
            locks.Remove(removedLock);
            OnLockRemoved.Invoke(this);
            Open();
        }

        public void Open()
        {
            if (this.isOpen || IsLocked || !allowedActions.Contains(LastInteraction.Type))
                return;

            this.isOpen = true;
            OnOpen.Invoke(this);
        }

        public void Close()
        {
            if (!this.isOpen)
                return;

            this.isOpen = false;
            OnClose.Invoke(this);
        }
    }
}