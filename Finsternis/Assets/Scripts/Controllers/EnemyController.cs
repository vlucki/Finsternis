using UnityEngine;
using Random = UnityEngine.Random;

namespace Finsternis
{
    [RequireComponent(typeof(Movement), typeof(Animator))]
    public class EnemyController : CharacterController
    {
        [SerializeField]
        [Range(1, 50)]
        private float _aggroRange = 2f;

        [SerializeField]
        [Range(1, 50)]
        private float _wanderCycle = 1f;

        [SerializeField]
        [Range(0, 1)]
        private float wanderFrequency = 0.5f;

        [SerializeField]
        private bool _ignoreWalls = false;

        [SerializeField]
        [Range(0, 10)]
        private float _reach = 0.5f;

        [SerializeField]
        private GameObject _target;

        [SerializeField]
        private float interestPersistence = 2f;

        private float timeSinceLastSawTarget = 0;
        private float timeSinceLastWander = 0;

        private Vector3 _targetLocation;

        private bool hasTarget;
        public GameObject ragdoll;

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
                        if (_target.GetComponent<CharacterController>().IsDead())
                        {
                            hasTarget = false;
                        }
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
                            Move((_target.transform.position - transform.position).normalized);
                        }
                        else
                        {
                            hasTarget = canSeeTarget;
                        }

                    }

                    if (!canSeeTarget && !hasTarget)
                    {
                        timeSinceLastSawTarget = 0;
                        Wander();
                    }

                }
            }
            if (GetComponent<Collider>().enabled && (IsDead() || IsDying()))
            {
                GetComponent<Collider>().enabled = false;
                GetComponent<Rigidbody>().isKinematic = true;
                if (ragdoll)
                {
                    Instantiate(ragdoll, transform.position, transform.rotation);
                    Destroy(gameObject);
                    gameObject.SetActive(false);
                }
            }
        }

        private void Wander()
        {
            timeSinceLastWander += Time.deltaTime;
            if (timeSinceLastWander >= _wanderCycle)
            {
                Move(GetWanderingDirection());
                timeSinceLastWander = 0;
            }
            else
            {
                Move(GetComponent<Movement>().Direction);
            }
        }

        private Vector3 GetWanderingDirection()
        {
            Vector3 dir = Vector3.zero;
            float value = Random.value;
            if (value > wanderFrequency)
                dir = new Vector3(Random.value * 10, 0, Random.value * 10);

            if (Random.value > 0.5f)
                dir.x *= -1;
            if (Random.value > 0.5f)
                dir.z *= -1;

            return dir;
        }

        private bool CheckRange()
        {
            return Vector3.Distance(_targetLocation, transform.position) <= _reach;
        }

        private bool LookForTarget()
        {
            bool canSeeTarget = false;

            if (!_ignoreWalls)
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

    }
}