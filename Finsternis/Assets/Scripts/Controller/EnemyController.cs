using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Movement), typeof(Animator))]
public class EnemyController : CharacterController
{

    [SerializeField]
    [Range(1, 50)]
    private float _aggroRange = 2f;

    [SerializeField]
    private bool _trackingIgnoreWalls = false;

    [SerializeField]
    [Range(0, 10)]
    private float _reach = 0.5f;

    [SerializeField]
    private GameObject _target;

    [SerializeField]
    private float interestPersistence = 2f;

    private float timeSinceLastSawTarget = 0;

    private Vector3 _targetLocation;

    private bool hasTarget;

    public override void Awake()
    {
        base.Awake();
        _target = GameObject.FindGameObjectWithTag("Player");
    }

    public override void Update()
    {
        base.Update();
        if (!IsDead())
        {
            if (!IsDying())
            {

                bool canSeeTarget = LookForTarget();
                if (!hasTarget)
                    hasTarget = canSeeTarget;
                else if (!canSeeTarget)
                {
                    timeSinceLastSawTarget += Time.deltaTime;
                    hasTarget = (timeSinceLastSawTarget >= interestPersistence); //stop trying to go towards the target
                }

                if (!IsAttacking())
                {
                    if (canSeeTarget && CheckRange())
                    {
                        float angle = Vector3.Angle(transform.forward, (_target.transform.position - transform.position));

                        if (angle < 30f)
                        {
                            Attack();
                        }
                    }
                    else if (hasTarget && transform.position != _targetLocation - GetOffset(_targetLocation))
                    {
                        Move();
                    }
                    else
                    {
                        hasTarget = canSeeTarget;
                    }

                }

                if (!canSeeTarget && !hasTarget)
                    GetComponent<Movement>().Direction = Vector2.zero;

            }
            else
            {
                GetComponent<Collider>().enabled = false;
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    private bool CheckRange()
    {
        return Vector3.Distance(_targetLocation, transform.position) <= _reach;
    }

    private bool LookForTarget()
    {
        bool canSeeTarget = false;
        Vector3 oldTargetLocation = _targetLocation;
        if (!_trackingIgnoreWalls)
        {
            RaycastHit hit;
            if (Physics.Raycast(
                transform.position + Vector3.up,
                _target.transform.position - transform.position,
                out hit,
                _aggroRange))
            {
                canSeeTarget = _target.Equals(hit.collider.gameObject);
                if (canSeeTarget)
                    _targetLocation = _target.transform.position;
            }
        }
        else
        {
            canSeeTarget = Vector3.Distance(transform.position, _target.transform.position) <= _aggroRange;
            if (canSeeTarget)
                _targetLocation = _target.transform.position;
        }

        return canSeeTarget;
    }

    private Vector3 GetOffset(Vector3 position)
    {
        Vector3 offset = (transform.position - position).normalized * _reach;
        offset.y = 0;
        return offset;
    }

    protected override void Move()
    {
        GetComponent<Movement>().Direction = (_target.transform.position - transform.position).normalized;

        base.Move();
    }

}
