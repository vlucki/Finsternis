namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;

    public class OpeneableEntity : Entity, IOpeneable
    {
        [Serializable]
        public struct OpeneableEvts
        {
            public EntityEvent onOpen;
            public EntityEvent onClose;
            public EntityEvent onLockRemoved;
        }

        [Header("OpeneableEntity fields")]
        [Space]

        [SerializeField]
        private OpeneableEvts openeableEvents;

        private bool isOpen;

        private List<Lock> locks;

        public bool IsLocked { get { return this.locks.Count > 0; } }

        public bool IsOpen { get { return this.isOpen; } }

        public OpeneableEvts OpeneableEvents { get { return this.openeableEvents; } }

        protected override void Awake()
        {
            base.Awake();
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
            OpeneableEvents.onLockRemoved.Invoke(this);
            Open();
        }

        public virtual void Open()
        {
            if (this.isOpen || IsLocked)
                return;

            this.isOpen = true;
            OpeneableEvents.onOpen.Invoke(this);
        }

        public virtual void Close()
        {
            if (!this.isOpen)
                return;

            this.isOpen = false;
            OpeneableEvents.onClose.Invoke(this);
        }
    }
}