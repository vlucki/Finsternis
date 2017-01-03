namespace Finsternis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(Collider))]
    public class Trigger : CustomBehaviour
    {
        [Serializable]
        public class OnTriggerEvent : Events.CustomEvent<GameObject> { }

        public OnTriggerEvent onEnter;
        public OnTriggerEvent onExit;

        public LayerMask layersToIgnore;
        [SerializeField]
        protected List<Collider> collidersToIgnore;

        [SerializeField]
        private List<TriggerConstraint> constraints;

        private HashSet<Collider> collidersWithin;

        protected
#if UNITY_EDITOR
            new
#endif
            Collider collider;

        protected virtual void Awake()
        {
            if (!collider)
                collider = GetComponent<Collider>();
            this.collidersWithin = new HashSet<Collider>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (ShouldTrigger(other))
            {
                this.collidersWithin.Add(other);
                if (onEnter)
                    onEnter.Invoke(other.gameObject);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (collidersWithin.Remove(other))
            {
                if (onExit)
                    onExit.Invoke(other.gameObject);
            }
        }

        protected void OnTriggerStay(Collider other)
        {
            if (ShouldTrigger(other))
            {
                if (this.collidersWithin.Add(other))
                {
                    if (onEnter)
                        onEnter.Invoke(other.gameObject);
                }
            }
            else if (collidersWithin.Remove(other))
            {
                if (onExit)
                    onExit.Invoke(other.gameObject);
            }
        }

        protected virtual bool ShouldTrigger(Collider other)
        {
            if (other.gameObject == this.gameObject || other.transform.IsChildOf(this.transform))
                return false;

            LayerMask otherLayer = 1 << other.gameObject.layer;
            if ((otherLayer & this.layersToIgnore) == otherLayer)
                return false;

            if (this.collidersToIgnore != null && this.collidersToIgnore.Contains(other))
                return false;

            if (this.constraints != null && this.constraints.Any((constraint) => !constraint.Check(this, other)))
                return false;

            return true;
        }

        public void Ignore(GameObject obj)
        {
            if (obj && obj != gameObject)
            {
                Collider c = obj.GetComponent<Collider>();
                if (c && !this.collidersToIgnore.Contains(c))
                    this.collidersToIgnore.Add(c);
            }
        }
    }
}