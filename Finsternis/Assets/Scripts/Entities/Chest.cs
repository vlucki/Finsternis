namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;

    [RequireComponent(typeof(Animator), typeof(Trigger))]
    public class Chest : Entity, IUnlockable, IOpeneable
    {
        private Animator animator;

        private List<KeyCard> locks;

        private HashSet<Entity> entitiesInRange;
        [System.Serializable]
        public class UnlockEvent : CustomEvent<int> { }
        [System.Serializable]
        public class OpenEvent : CustomEvent<Chest> { }

        [Header("Chest Events", order = 0)]
        public UnlockEvent OnLockRemoved;
        public OpenEvent OnOpen;

        public bool IsOpen { get; private set; }

        public bool IsLocked { get { return (locks != null && locks.Count > 0); } }

        protected override void Awake()
        {
            base.Awake();
            this.animator = GetComponent<Animator>();
            this.entitiesInRange = new HashSet<Entity>();
        }

        public void TryOpening()
        {
            var action = LastInteraction as OpenAction;
            if (!action)
                return;
            if (!entitiesInRange.Contains(action.Agent))
                return;

            if (this.IsLocked)
                RemoveLock(action.KeyCard);

            if (this.IsLocked) //if it's still locked, just return
                return;

            Open();
        }

        public void Open()
        {
            if (LastInteraction)
            {
                var inv = LastInteraction.GetComponent<Inventory>();
                if(inv)
                    inv.AddCard(CardFactory.MakeCard());
            }
            interactable = false;
            this.animator.SetTrigger("Open");
            this.IsOpen = true;
            OnOpen.Invoke(this);
        }

        public void Close()
        {
            this.IsOpen = false;
        }


        public void AddLock(KeyCard c)
        {
            locks.Add(c);
        }

        public void RemoveLock(KeyCard l)
        {
            if (!l || !locks.Remove(l))
                return;
            OnLockRemoved.Invoke(locks.Count);
        }

        public void AddEntityInRange(GameObject obj)
        {
            var entity = obj.GetComponent<Entity>();
            if (entity)
                entitiesInRange.Add(entity);
        }

        public void RemoveEntityInRange(GameObject obj)
        {
            var entity = obj.GetComponent<Entity>();
            if (entity)
                entitiesInRange.Remove(entity);
        }
    }
}