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
        public class OnTriggerEvent : UnityEvent<GameObject> { }

        public OnTriggerEvent onEnter;
        public OnTriggerEvent onExit;

        public LayerMask ignoreLayers;
        [SerializeField]
        protected List<Collider> ignoreColliders;

        protected new Collider collider;

        private GameObject _objectEntered;
        private GameObject _objectExited;

        public GameObject ObjectEntered { get { return _objectEntered; } }
        public GameObject ObjectExited { get { return _objectExited; } }

        protected virtual void Awake()
        {
            if (!collider)
                collider = GetComponent<Collider>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (ShouldTrigger(other))
            {
                _objectEntered = other.gameObject;
                onEnter.Invoke(_objectEntered);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (ShouldTrigger(other))
            {
                _objectExited = other.gameObject;
                onExit.Invoke(_objectExited);
            }
        }

        protected virtual bool ShouldTrigger(Collider other)
        {
            if (other.gameObject == this.gameObject || other.transform.IsChildOf(this.transform))
                return false;

            LayerMask otherLayer = 1 << other.gameObject.layer;
            if ((otherLayer & ignoreLayers) == otherLayer)
                return false;

            if (ignoreColliders != null && ignoreColliders.Contains(other))
                return false;

            return true;
        }

        public void Ignore(GameObject obj)
        {
            if (obj && obj != gameObject)
            {
                Collider c = obj.GetComponent<Collider>();
                if (c && !ignoreColliders.Contains(c))
                    ignoreColliders.Add(c);
            }
        }
    }
}