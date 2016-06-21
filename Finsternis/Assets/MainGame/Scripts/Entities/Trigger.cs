using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class OnTriggerEvent : UnityEvent<GameObject> { }

[RequireComponent(typeof(Collider))]
public class Trigger : MonoBehaviour
{
    public OnTriggerEvent onEnter;
    public OnTriggerEvent onExit;

    private GameObject _objectEntered;
    private GameObject _objectExited;

    public GameObject ObjectEntered { get { return _objectEntered; } }
    public GameObject ObjectExited { get { return _objectExited; } }

    public LayerMask ignoreLayers;
    public List<Collider> ignoreColliders;

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

    private bool ShouldTrigger(Collider other)
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
        if(obj && obj != gameObject)
        {
            Collider c = obj.GetComponent<Collider>();
            if (c && !ignoreColliders.Contains(c))
                ignoreColliders.Add(c);
        }
    }
}
