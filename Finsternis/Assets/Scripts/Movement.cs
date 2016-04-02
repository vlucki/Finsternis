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
    [Range(0, 1000)]
    private float speed = 3;
    [SerializeField]
    private Vector2 maxVelocity = new Vector2(3, 3);
    [SerializeField]
    protected Rigidbody rigidBody;

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
        if(!rigidBody)
            rigidBody = GetComponent<Rigidbody>();
    }

    private void GenerateRigidBody()
    {
        rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.drag = 1f;
    }

    protected virtual void FixedUpdate()
    {
        if (direction != Vector3.zero)
        {
            rigidBody.AddForce(direction * speed, ForceMode.VelocityChange);
        }

        LimitVelocity();
    }

    private void LimitVelocity()
    {
        float velX = Mathf.Abs(rigidBody.velocity.x);
        float velZ = Mathf.Abs(rigidBody.velocity.z);
        Vector2 acceleration = GetAcceleration();

        if (velX > maxVelocity.x || velZ > maxVelocity.y)
        {
            velX = GetFinalValue(velX, rigidBody.velocity.x, maxVelocity.x);
            velZ = GetFinalValue(velZ, rigidBody.velocity.z, maxVelocity.y);
            rigidBody.velocity = new Vector3(velX, rigidBody.velocity.y, velZ);
            rigidBody.AddForce(-acceleration);
        }
        lastVelocity = rigidBody.velocity;
    }

    private Vector2 GetAcceleration()
    {
        return (rigidBody.velocity - lastVelocity) / Time.fixedDeltaTime;
    }

    //checks whether the new absolute velocity in a given axis is greater than the top speed of that axis picking the top speed if that is the case
    //then, if the old value was negative, multiply the new one by -1 so it stays negative (and the body doesn't start switching directions when it shouldn't)
    float GetFinalValue(float newAbsoluteValue, float oldSignaledValue, float limit)
    {
        return Mathf.Min(newAbsoluteValue, limit) * (oldSignaledValue < 0 ? -1 : 1);
    }
}
