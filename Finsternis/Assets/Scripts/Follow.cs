using UnityEngine;
using System.Collections;

public class Follow
    : Movement
{
    public Transform target;

    [Tooltip("The max distance the target will be tracked. A value of 0 means and infinite distance.")]
    [Range(0, 100)]
    public float range = 0;
    [Tooltip("The distance that should be kept from the target. Higher values result in more distance from the target.")]
    [Range(0, 100)]
    public float closeRange = 0;


    protected override void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > closeRange && (range == 0 || distance <= range))
            Direction = target.position - transform.position;
        else if(Direction != Vector3.zero)
        {
            Direction = Vector2.zero;
        }

        base.FixedUpdate();
    }
}
