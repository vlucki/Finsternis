using UnityEngine;
using System.Collections;

public class Follow
    : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = Vector3.one;

    [Tooltip("How far the target can get before it should be followed.")]
    [Range(0, 1)]
    public float interpolation = 0.2f;


    void FixedUpdate()
    {
        Vector3 idealPosition = target.position - offset;
        transform.position = Vector3.Lerp(transform.position, idealPosition, interpolation);
    }
}
