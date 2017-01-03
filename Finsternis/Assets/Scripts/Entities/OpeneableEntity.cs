namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;

    public class OpeneableEntity : Entity, IOpeneable
    {
        public static readonly int LockedBool = Animator.StringToHash("locked");
        public static readonly int OpenBool = Animator.StringToHash("open");

        [Serializable]
        public class OpeneableEvent : Events.CustomEvent<IOpeneable> { }

        [EasyEditor.Inspector(group = "OpeneableEntity", foldable = true)]
        public OpeneableEvent onOpen;
        public OpeneableEvent onClose;

        private List<Lock> locks;

        public bool IsOpen { get; private set; }

        public bool IsLocked { get { return locks.Count > 0; } }

        public System.Collections.ObjectModel.ReadOnlyCollection<Lock> Locks { get { return this.locks.AsReadOnly(); } }

        protected override void Awake()
        {
            base.Awake();
            this.locks = new List<Lock>();
        }

        public void AddLock(Lock newLock)
        {
            this.locks.Add(newLock);
            newLock.onUnlock += l => this.locks.Remove(l);
        }

        public virtual void Open()
        {
            if (this.IsOpen || this.IsLocked)
                return;

            this.IsOpen = true;
            onOpen.Invoke(this);
        }

        public virtual void Close()
        {
            if (!this.IsOpen)
                return;

            this.IsOpen = false;
            onClose.Invoke(this);
        }
    }
}