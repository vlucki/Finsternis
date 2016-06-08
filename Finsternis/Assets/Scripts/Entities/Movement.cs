using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(Rigidbody))]

public class Movement : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1f)]
    private float _speed = 0.175f;

    [SerializeField]
    [Range(1, 10)]
    private float _maxVelocity = 2;

    private Vector3 _direction;

    protected Rigidbody body;

    public float Speed
    {
        get { return _speed; }
        set { _speed = Mathf.Max(0, value); }
    }

    public Vector3 Direction
    {
        get { return _direction; }
        set { _direction = value.normalized; }
    }
    
    protected virtual void Start()
    {
        if(!body)
            body = GetComponent<Rigidbody>();
    }

    protected virtual void FixedUpdate()
    {
        if (_direction != Vector3.zero)
        {
            body.AddForce(_direction * _speed, ForceMode.VelocityChange);
            if (GetHorizontalSpeed() > _maxVelocity)
                body.AddForce(-_direction * Speed, ForceMode.VelocityChange);
        }
    }

    public float GetSpeed()
    {
        return body.velocity.magnitude;
    }

    internal float GetHorizontalSpeed()
    {
        if(body)
            return Mathf.Pow(body.velocity.x, 2) + Mathf.Pow(body.velocity.z, 2);
        return 0;
    }
}
