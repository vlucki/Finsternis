using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public float aggroRange = 2f;

    private Animator _characterAnimator;
    private Movement _characterMovement;

    private GameObject target;

    bool targetOnRange = false;

    void Start()
    {
        _characterMovement = GetComponent<Movement>();
        _characterAnimator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player");
    }
    
    void Update()
    {
        targetOnRange = Vector3.Distance(transform.position, target.transform.position) <= aggroRange;
        if (targetOnRange)
        {
            transform.LookAt(target.transform);
            //Quaternion v = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(transform.forward, (target.transform.position - transform.position).normalized), 0.25f);
            //transform.rotation = v;
        }
    }
}
