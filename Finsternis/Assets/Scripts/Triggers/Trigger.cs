using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Finsternis
{
    [RequireComponent(typeof(Collider))]
    public class Trigger : MonoBehaviour
    {
        [Serializable]
        public class OnTriggerEvent : CustomEvent<GameObject> { }

        public OnTriggerEvent onEnter;
        public OnTriggerEvent onExit;

        public LayerMask layersToIgnore;
        [SerializeField]
        protected List<Collider> collidersToIgnore;

        protected new Collider collider;

        private GameObject objectEntered;
        private GameObject objectExited;

        public GameObject ObjectEntered { get { return this.objectEntered; } }
        public GameObject ObjectExited  { get { return this.objectExited; } }

        protected virtual void Awake()
        {
            if (!collider)
                collider = GetComponent<Collider>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (ShouldTrigger(other))
            {
                this.objectEntered = other.gameObject;
                if(onEnter)
                    onEnter.Invoke(objectEntered);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (ShouldTrigger(other))
            {
                this.objectExited = other.gameObject;
                if(onExit)
                    onExit.Invoke(objectExited);
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