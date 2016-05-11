using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Movement), typeof(Animator))]
public class EnemyController : CharacterController {

    [SerializeField]
    [Range(1, 50)]
    private float _aggroRange = 2f;


    [SerializeField]
    [Range(0, 10)]
    private float _reach = 0.5f;

    [SerializeField]
    private GameObject _target;

    bool followTarget = false;
    bool targetOnRange = false;

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
                float dist = Vector3.Distance(transform.position, _target.transform.position);
                followTarget = dist <= _aggroRange;
                if (followTarget)
                {
                    targetOnRange = dist <= _reach;
                    if (!IsAttacking())
                    {
                        float angle = Vector3.Angle(transform.forward, (_target.transform.position - transform.position));

                        if (targetOnRange && angle < 30f)
                        {
                            Attack();
                        }
                        else
                        {
                            Move();
                        }
                    }
                }
                
            }
            else
            {
                GetComponent<Collider>().enabled = false;
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    protected override void Move()
    {
        GetComponent<Movement>().Direction = (_target.transform.position - transform.position).normalized;

        base.Move();
    }

}
