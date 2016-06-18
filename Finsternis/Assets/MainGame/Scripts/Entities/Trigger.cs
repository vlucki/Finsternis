using System;
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

    public LayerMask layerToCheck;   

    void OnTriggerEnter(Collider other)
    {
        if (ShouldTrigger(other))
        {
            _objectEntered = other.gameObject;
            onEnter.Invoke(_objectEntered);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (ShouldTrigger(other))
        {
            _objectExited = other.gameObject;
            onExit.Invoke(_objectExited);
        }
    }

    private bool ShouldTrigger(Collider other)
    {
        LayerMask otherLayer = 1 << other.gameObject.layer;
        
        return other.gameObject != this.gameObject && !other.transform.IsChildOf(this.transform) && ((otherLayer & layerToCheck) == otherLayer);
    }
}
