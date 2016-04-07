using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Movement), typeof(Animator))]
public class EnemyController : CharacterController {

    [Range(1, 10)]
    public float aggroRange = 2f;

    [SerializeField]
    private GameObject target;

    bool targetOnRange = false;

    public override void Awake()
    {
        base.Awake();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    public override void Update()
    {
        if (!IsDead())
        {
            if (!IsDying())
            {
                targetOnRange = Vector3.Distance(transform.position, target.transform.position) <= aggroRange;
                if (targetOnRange)
                {
                    transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                }
            }
            else
            {
                GetComponent<Collider>().enabled = false;
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }
        else
        {
            //Destroy(gameObject);
        }
    }

}
