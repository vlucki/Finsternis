namespace Finsternis
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityQuery;
    using Random = UnityEngine.Random;

    [RequireComponent(typeof(EnemyChar), typeof(LookForTargetAction))]
    public class EnemyController : CharController
    {
        [SerializeField, Range(1, 50)]
        private float wanderCycle = 1f;

        [SerializeField, Range(0, 1)]
        private float wanderFrequency = 0.5f;

        [SerializeField, Range(0, 10)]
        private float reach = 0.5f;

        [SerializeField]
        private float interestPersistence = 2f;

        private LookForTargetAction lookForTarget;

        private Coroutine wanderingRoutine;

        bool wandering;

        protected override void Awake()
        {
            base.Awake();
            this.lookForTarget = GetComponent<LookForTargetAction>();
            if(this.lookForTarget)
                this.lookForTarget.Activate();
        }

        protected override void OnCharacterDeath()
        {
            if (this.lookForTarget)
            {
                base.OnCharacterDeath();
                this.lookForTarget.Deactivate();
            }
        }

        protected override void DoUpdate()
        {
            base.DoUpdate();
            if (this.lookForTarget && this.lookForTarget.isActiveAndEnabled && this.lookForTarget.CurrentTarget)
            {
                TrackTarget();
            }
            else if(this.wanderingRoutine == null)
            {
                this.wanderingRoutine = StartCoroutine(_Wander());
            }
        }

        private void TrackTarget()
        {
            var tgtPos = this.lookForTarget.CurrentTarget.transform.position;
            bool inRange = tgtPos.Distance(this.transform.position) <= this.reach;
            if (inRange)
                Attack();
            else
                SetDirection(tgtPos - this.transform.position);
        }

        private IEnumerator _Wander()
        {
            float elapsed = this.wanderCycle;
            Vector3 direction = Vector3.zero;
            while (!HasTarget())
            {
                elapsed += Time.deltaTime;
                if (elapsed >= this.wanderCycle)
                {
                    direction = (GetWanderingDirection());
                    elapsed = 0;
                }
                SetDirection(direction);
                yield return null;
            }

            this.wanderingRoutine = null;
        }

        private bool HasTarget()
        {
            return this.lookForTarget && this.lookForTarget.CurrentTarget;
        }

        private Vector3 GetWanderingDirection()
        {
            Vector3 dir = Vector3.zero;
            float value = Random.value;
            if (value < wanderFrequency)
            {
                dir = new Vector3(Random.value * 10, 0, Random.value * 10);

                if (Random.value > 0.5f)
                    dir.x *= -1;
                if (Random.value > 0.5f)
                    dir.z *= -1;
            }
            return dir;
        }

    }
}