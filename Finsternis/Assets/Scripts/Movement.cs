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
    [Range(0, 10000)]
    private float speed = 500;
    [SerializeField]
    private Vector2 maxVelocity = new Vector2(3, 3);
    [SerializeField]
    protected Rigidbody body;

    private Vector3 direction;
    private Vector3 lastVelocity;

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
            body.AddForce(direction * speed);
        }

        LimitVelocity();
    }

    private void LimitVelocity()
    {
        float velX = Mathf.Abs(body.velocity.x);
        float velZ = Mathf.Abs(body.velocity.z);
        Vector2 acceleration = GetAcceleration();

        if (velX > maxVelocity.x || velZ > maxVelocity.y)
        {
            velX = GetFinalValue(velX, body.velocity.x, maxVelocity.x);
            velZ = GetFinalValue(velZ, body.velocity.z, maxVelocity.y);
            body.velocity = new Vector3(velX, body.velocity.y, velZ);
            body.AddForce(-acceleration);
        }
        lastVelocity = body.velocity;
    }

    private Vector2 GetAcceleration()
    {
        return (body.velocity - lastVelocity) / Time.fixedDeltaTime;
    }

    //checks whether the new absolute velocity in a given axis is greater than the top speed of that axis picking the top speed if that is the case
    //then, if the old value was negative, multiply the new one by -1 so it stays negative (and the body doesn't start switching directions when it shouldn't)
    float GetFinalValue(float newAbsoluteValue, float oldSignaledValue, float limit)
    {
        return Mathf.Min(newAbsoluteValue, limit) * (oldSignaledValue < 0 ? -1 : 1);
    }
}
