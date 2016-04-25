using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(Rigidbody))]

public class Movement : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1f)]
    private float _speed = 0.175f;

    private Vector3 _direction;

    protected Rigidbody body;

    public Vector3 Direction
    {
        get { return _direction; }
        set { _direction = value.normalized; }
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
        if (_direction != Vector3.zero)
        {
            body.AddForce(_direction * _speed, ForceMode.VelocityChange);
        }
    }

    public float GetSpeed()
    {
        return body.velocity.magnitude;
    }

    internal float GetHorizontalSpeed()
    {
        return Mathf.Pow(body.velocity.x, 2) + Mathf.Pow(body.velocity.z, 2);
    }
}
