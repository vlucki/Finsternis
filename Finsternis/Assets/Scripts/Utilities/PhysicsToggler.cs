using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityQuery;

public class PhysicsToggler : MonoBehaviour
{
    [SerializeField]
    private string referenceObjectTag;

    [SerializeField]
    [Range(1, 99)]
    private float distanceThreshold = 20;

    private bool wasInRange = true;

    private Transform transformToObserve;

    private List<Rigidbody> rigidbodies;
    private List<Collider> colliders;

    void Awake()
    {
        var obj = GameObject.FindGameObjectWithTag(referenceObjectTag);
        if (obj)
            this.transformToObserve = obj.transform;
    }

    void Start()
    {
        this.rigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        this.colliders = new List<Collider>(GetComponentsInChildren<Collider>());
        if (this.rigidbodies.IsNullOrEmpty() && this.colliders.IsNullOrEmpty())
            this.Disable();
    }

    void FixedUpdate()
    {
        if (!this.transformToObserve)
            this.Disable();
        else
        {
            bool inRange = this.transformToObserve.position.Distance(this.transform.position) <= this.distanceThreshold;
            if (!(inRange && this.wasInRange))
            {
                this.wasInRange = inRange;
                this.rigidbodies.ForEach(body => Toggle(body, inRange));
                this.colliders.ForEach(col => Toggle(col, inRange));
            }
        }
    }

    private void Toggle(Rigidbody body, bool active)
    {
        if (!active)
            body.Sleep();
        body.detectCollisions = active;
    }

    private void Toggle(Collider col, bool enabled)
    {
        col.enabled = enabled;
        return;
    }
}
