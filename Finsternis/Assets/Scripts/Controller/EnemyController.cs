using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Movement), typeof(Animator))]
public class EnemyController : CharacterController {

    [SerializeField]
    [Range(1, 10)]
    private float _aggroRange = 2f;

    [SerializeField]
    private GameObject _target;

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
                targetOnRange = Vector3.Distance(transform.position, _target.transform.position) <= _aggroRange;
                if (targetOnRange && !IsAttacking())
                {
                    Move();
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
