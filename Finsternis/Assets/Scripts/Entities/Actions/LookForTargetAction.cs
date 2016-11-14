namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityQuery;
    using UnityEngine.Events;
    using System.Collections;
    using System;

    [RequireComponent(typeof(MovementAction))]
    public class LookForTargetAction : EntityAction
    {
        [System.Serializable]
        public class TargetChangedEvent : CustomEvent<GameObject> { }

        [SerializeField]
        [Range(0, 50)]
        [Tooltip("How far away will targets be detected.")]
        private float detectionRange = 4;

        [SerializeField]
        [Range(1, 51)]
        [Tooltip("How far away will targets persist before being forgot.")]
        private float persistenceRange = 10;

        [SerializeField]
        private string tagToConsider = "Enemy";

        [SerializeField]
        private bool ignoreObstacles = false;

        public TargetChangedEvent onTargetChanged;

        private Character lastTarget;
        private Character currentTarget;

        private List<Character> entitiesInRange;

        private MovementAction movement;

        private Coroutine running;

        public Character CurrentTarget
        {
            get
            {
                return this.currentTarget;
            }
        }

        public Character LastTarget
        {
            get { return this.lastTarget; }
        }

        protected override void Awake()
        {
            base.Awake();
            this.movement = GetComponent<MovementAction>();
            this.entitiesInRange = new List<Character>();
        }

        public void Activate()
        {
            if (this.running != null)
                return;
            this.running = this.StartCoroutine(_LookForTargets());
        }

        public void Deactivate()
        {
            if (this.running == null)
                return;
            this.StopCoroutine(this.running);
            this.running = null;

            SetTarget(null);
        }

        private IEnumerator _LookForTargets()
        {
            while (true)
            {
                yield return Wait.Fixed();

                UpdateCurrentTarget();

                if(!this.currentTarget)
                    FindTargets();

                if (this.currentTarget)
                {
                    this.movement.ShouldFaceMovementDirection = false;
                    this.movement.FacingDirection = currentTarget.transform.position.WithY(0) - this.transform.position.WithY(0);
                }
            }
        }

        private void UpdateCurrentTarget()
        {
            if (!this.currentTarget || this.currentTarget.Dead)
            {
                SetTarget(null);
            }
            else if (this.currentTarget)
            {
                var dist = this.transform.position.WithY(0).Distance(this.currentTarget.transform.position.WithY(0));
                if (dist > this.persistenceRange)
                    SetTarget(null);
                else if(!ignoreObstacles) //check if there's anything obtructing the view of the target
                {
                    var origin = transform.position.WithY(1) + transform.forward;
                    if (Physics.SphereCast(
                        new Ray(origin, transform.forward),
                        .3f, dist,
                        1 << (1 ^ (LayerMask.NameToLayer("Entity")))))
                    {
                        SetTarget(null);
                    }
                }
            }
        }

        protected virtual void FindTargets()
        {
            var collidersInRange = new List<Collider>(Physics.OverlapSphere(transform.position, this.detectionRange, 1 << LayerMask.NameToLayer("Entity")));
            this.entitiesInRange.Clear();
            var closest = this.currentTarget;

            foreach (var col in collidersInRange)
            {
                var entity = col.GetComponent<Character>();
                if (entity && entity != this.agent && !entity.Dead && entity.CompareTag(this.tagToConsider))
                {
                    entitiesInRange.Add(entity);
                    var pos = transform.position;
                    if (!closest || 
                       closest.transform.position.Distance(pos) > entity.transform.position.Distance(pos))
                        closest = entity;
                }
            }
            if (this.entitiesInRange.IsNullOrEmpty())
                SetTarget(null);
            else
            {
                if (!this.currentTarget || !this.entitiesInRange.Contains(this.currentTarget))
                {
                    SetTarget(closest);
                }
            }
        }

        private void SetTarget(Character target)
        {
            this.lastTarget = this.currentTarget;
            this.currentTarget = target;
            this.movement.ShouldFaceMovementDirection = !target;

            if (this.currentTarget != this.lastTarget)
                onTargetChanged.Invoke(this.currentTarget ? this.currentTarget.gameObject : null);
        }

        public void Toggle()
        {
            if (this.running != null)
                Deactivate();
            else
                Activate();
        }
    }
}