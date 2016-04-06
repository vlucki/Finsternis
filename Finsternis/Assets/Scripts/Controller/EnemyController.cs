using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Movement), typeof(Animator))]
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
        GetComponent<Character>().death += EnemyController_death;
        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void EnemyController_death()
    {
        _characterAnimator.SetBool("dying", true);
    }

    void Update()
    {
        if (!_characterAnimator.GetBool("dead"))
        {
            if (!_characterAnimator.GetBool("dying"))
            {
                targetOnRange = Vector3.Distance(transform.position, target.transform.position) <= aggroRange;
                if (targetOnRange)
                {
                    transform.LookAt(target.transform);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
