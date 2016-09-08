using UnityEngine;
using UnityQuery;
using Random = UnityEngine.Random;

namespace Finsternis
{
    [RequireComponent(typeof(EnemyChar))]
    public class EnemyController : CharController
    {
        [SerializeField]
        [Range(1, 50)]
        private float aggroRange = 2f;

        [SerializeField]
        [Range(1, 50)]
        private float _wanderCycle = 1f;

        [SerializeField]
        [Range(0, 1)]
        private float wanderFrequency = 0.5f;

        [SerializeField]
        private bool ignoreWalls = false;

        [SerializeField]
        [Range(0, 10)]
        private float reach = 0.5f;

        [SerializeField]
        private GameObject target;

        [SerializeField]
        private float interestPersistence = 2f;

        private float timeSinceLastSawTarget = 0;
        private float timeSinceLastWander = 0;

        private Vector3 targetLocation;

        private bool hasTarget;
        public GameObject ragdoll;

        public override void Awake()
        {
            base.Awake();
            target = GameObject.FindGameObjectWithTag("Player");
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
                        if (target.GetComponent<CharController>().IsDead())
                        {
                            hasTarget = false;
                        }
                    }

                    if (!IsAttacking())
                    {
                        if (canSeeTarget && CheckRange())
                        {
                            float angle = Vector3.Angle(transform.forward, (target.transform.position - transform.position));

                            if (angle < 30f)
                            {
                                Attack();
                            }
                        }
                        else if (hasTarget && transform.position != targetLocation - GetOffset(targetLocation))
                        {
                            SetDirection((target.transform.position - transform.position).normalized);
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
                foreach (var c in GetComponentsInChildren<Collider>())
                    c.enabled = false;
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
                SetDirection(GetWanderingDirection());
                timeSinceLastWander = 0;
            }
            else
            {
                SetDirection(characterMovement.Direction);
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
            return this.targetLocation.Distance(transform.position) <= this.reach;
        }

        private bool LookForTarget()
        {
            bool canSeeTarget = false;
            if (!this.target)
                return false;

            if (!ignoreWalls)
            {
                RaycastHit hit;
                if (Physics.Raycast(
                    transform.position + Vector3.up,
                    this.target.transform.position - transform.position,
                    out hit,
                    aggroRange))
                {
                    canSeeTarget = this.target.Equals(hit.collider.gameObject);
                }
            }
            else
            {
                canSeeTarget = transform.position.Distance(target.transform.position) <= aggroRange;
            }
            if (canSeeTarget)
                this.targetLocation = target.transform.position;
            return canSeeTarget;
        }

        private Vector3 GetOffset(Vector3 position)
        {
            Vector3 offset = (transform.position - position).normalized * reach;
            offset.y = 0;
            return offset;
        }

    }
}