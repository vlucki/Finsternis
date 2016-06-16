using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Trigger : MonoBehaviour
{
    public UnityEvent onEnter;
    public UnityEvent onExit;

    private Collider _objectEntered;
    private Collider _objectExited;

    public Collider ObjectEntered { get { return _objectEntered; } }
    public Collider ObjectExited { get { return _objectExited; } }

    public LayerMask layerToCheck;
   

    void OnTriggerEnter(Collider other)
    {
        if (ShouldTrigger(other))
        {
            _objectEntered = other;
            onEnter.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (ShouldTrigger(other))
        {
            _objectExited = other;
            onExit.Invoke();
        }
    }

    private bool ShouldTrigger(Collider other)
    {
        LayerMask otherLayer = 1 << other.gameObject.layer;
        
        return other.gameObject != this.gameObject && !other.transform.IsChildOf(this.transform) && ((otherLayer & layerToCheck) == otherLayer);
    }
}
