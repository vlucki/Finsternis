using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(Rigidbody))]

public class Movement : MonoBehaviour
{
    /*NOTES:
            The BEST way to achieve a "smooth stop" effect is to set a high(ish) LINEAR DRAG and then just crank up the speed modifier.
            Also, in order to avoid the gameobject from flying around at lightspeed, just set a top speed
        */
    [SerializeField]
    [Range(0, 1f)]
    private float speed = 0.175f;

    protected Rigidbody body;

    private Vector3 direction;

    public Vector3 Direction
    {
        get { return direction; }
        set { direction = value.normalized; }
    }

    // Use this for initialization
    protected virtual void Start()
    {
        if(!body)
            body = GetComponent<Rigidbody>();
    }

    private void GenerateRigidBody()
    {
        body = gameObject.AddComponent<Rigidbody>();
        body.drag = 1f;
    }

    protected virtual void FixedUpdate()
    {
        if (direction != Vector3.zero)
        {
            body.AddForce(direction * speed, ForceMode.VelocityChange);
        }
    }

    public float GetSpeed()
    {
        return body.velocity.magnitude;
    }
}
