using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[ExecuteInEditMode]
public class Follow : MonoBehaviour
{
    [SerializeField]
    private bool focusTarget = true;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform _originalTarget;
    [SerializeField]
    private float _tempTargetFocusTime = 0.3f;

    private float _timeFocusingTempTarget = 0;

    public UnityEvent OnTargetReached;

    [Header("Lock Rotation Axis")]
    public bool x;
    public bool y;
    public bool z;

    public Vector3 offset = Vector3.back + Vector3.up;

    private Vector3 _originalOffset;

    [Range(0, 1)]
    public float translationInterpolation = 0.1f;

    [Range(0, 1)]
    public float rotationInterpolation = 0.2f;

    public Vector3 OriginalOffset { get { return _originalOffset; } }

    public bool WasOffsetChanged { get { return offset == _originalOffset; } }

    public Transform Target { get { return this._target; } }

    void Awake()
    {
        _originalOffset = offset;
        _originalTarget = _target;
    }

    public void SetTarget(Transform target)
    {
        this._originalTarget = target;
        this._target = target;
    }

    public void SetTemporaryTarget(Transform target)
    {
        if (this._originalTarget == null)
            this._originalTarget = this._target;
        this._target = target;
        _timeFocusingTempTarget = 0;
    }

    public void ResetOffset()
    {
        offset = _originalOffset;
    }

    public bool FocusingTemporaryTarget()
    {
        return this._originalTarget != this._target;
    }

    void FixedUpdate()
    {
        Vector3 idealPosition = _target.position + offset;
        transform.position = Vector3.Lerp(transform.position, idealPosition, translationInterpolation);

        if (Vector3.Distance(idealPosition, transform.position) <= 0.1f)
        {
            OnTargetReached.Invoke();
            if(_tempTargetFocusTime > 0 && FocusingTemporaryTarget())
                _timeFocusingTempTarget += Time.deltaTime;
        }

        if (_timeFocusingTempTarget >= _tempTargetFocusTime)
            _target = _originalTarget;

        if (focusTarget && !(x && y && z))
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            Vector3 rotation = transform.eulerAngles;
            transform.forward = Vector3.Slerp(transform.forward, direction, rotationInterpolation);
            Vector3 newRotation = transform.eulerAngles;
            if (x)
                newRotation.x = rotation.x;
            if (y)
                newRotation.y = rotation.y;
            if (z)
                newRotation.z = rotation.z;
            transform.eulerAngles = newRotation;
        }
    }
}
