using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Follow
    : MonoBehaviour
{
    public Transform target;
    public bool focusTarget = true;

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

    public bool OffsetReset { get { return offset == _originalOffset; } }

    void Awake()
    {
        _originalOffset = offset;
    }

    public void ResetOffset()
    {
        offset = _originalOffset;
    }
    
    void FixedUpdate()
    {
        Vector3 idealPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, idealPosition, translationInterpolation);

        if (focusTarget && !(x && y && z))
        {
            Vector3 direction = (target.position - transform.position).normalized;
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
