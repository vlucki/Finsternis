using UnityEngine;
using UnityQuery;

[RequireComponent (typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1f)]
    private float _acceleration = 0.175f;

    [SerializeField]
    [Range(1, 10)]
    private float _maxVelocity = 2;

    [SerializeField]
    [Range(0, 1)]
    private float _minVelocityThreshold = 0.1f;

    private float _currentVelocity;
    private float _lastVelocity;

    private Vector3 _direction;

    private Vector3 _lastDirection;

    protected Rigidbody body;

    public float Speed
    {
        get { return _acceleration; }
        set { _acceleration = Mathf.Max(0, value); }
    }

    public Vector3 Direction
    {
        get { return _direction; }
    }

    public Vector3 LastDirection { get { return _lastDirection; } }
    
    public void SetDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
            direction.Normalize();
        this._direction = direction;
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
            _currentVelocity = VelocityNoY();
            if (_currentVelocity < _minVelocityThreshold && _currentVelocity < _lastVelocity && Vector3.Angle(_direction, _lastDirection) == 0)
            {
                _direction = Vector3.zero;
            }
            else
            {
                _lastDirection = _direction;
                _lastVelocity = _currentVelocity;
                if (_currentVelocity > _maxVelocity)
                    body.AddForce(-_direction * _acceleration, ForceMode.VelocityChange);
                else
                    body.AddForce(_direction * _acceleration, ForceMode.VelocityChange);
            }
        } else
        {
            _lastVelocity = _currentVelocity = 0;
        }
    }

    public float GetSpeed()
    {
        return body.velocity.magnitude;
    }

    internal float VelocityNoY()
    {
        if(body)
            return body.velocity.XZ().sqrMagnitude;
        return 0;
    }
}
