namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using System.Collections.Generic;
    using UnityQuery;

    [RequireComponent(typeof(MovementAction))]
    public class TargetLockAction : EntityAction
    {
        [SerializeField][Range(0, 10)]
        private float range = 4;

        [SerializeField]
        private GameObject targetMarkerPrefab;

        private GameObject targetMarker;

        private Character lastTarget;
        private Character currentTarget;

        private List<Character> entitiesInRange;

        private MovementAction movement;

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

        void OnEnable()
        {
            FindTargets();
        }

        void OnDisable()
        {
            DisableMarker();
            this.movement.ShouldFaceDirection = true;
        }

        void Update()
        {
            if (this.currentTarget)
            {
                this.movement.ShouldFaceDirection = false;
                this.movement.FacingDirection = currentTarget.transform.position.WithY(0) - this.transform.position.WithY(0);
            }
        }

        void FixedUpdate()
        {
            FindTargets();
            UpdateCurrentTarget();
        }

        private void UpdateCurrentTarget()
        {
            if (!this.currentTarget || this.currentTarget.Dead)
            {
                this.currentTarget = null;
                this.movement.ShouldFaceDirection = true;
                DisableMarker();
                return;
            }
            if (this.currentTarget)
            {
                var dist = this.transform.position.Distance(this.currentTarget.transform.position);
                if (dist > this.range)
                    this.currentTarget = null;
                else
                {
                    var origin = transform.position.WithY(1) + transform.forward;
                    if (Physics.SphereCast(
                        new Ray(origin, transform.forward),
                        .3f, dist,
                        1 << (1 ^ (LayerMask.NameToLayer("Entity")))))
                    {
                        this.currentTarget = null;
                    }
                }
            }
        }

        private void DisableMarker()
        {
            if (this.targetMarker)
            {
                this.targetMarker.transform.SetParent(null);
                this.targetMarker.SetActive(false);
            }
        }

        protected virtual void FindTargets()
        {
            var collidersInRange = new List<Collider>(Physics.OverlapSphere(transform.position, this.range, 1 << LayerMask.NameToLayer("Entity")));
            this.entitiesInRange.Clear();
            var closest = this.currentTarget;

            foreach (var col in collidersInRange)
            {
                var entity = col.GetComponent<Character>();
                if (entity && entity != this.agent)
                {
                    entitiesInRange.Add(entity);
                    var pos = transform.position;
                        if (!closest || closest.transform.position.Distance(pos) > entity.transform.position.Distance(pos))
                            closest = entity;
                }
            }
            if (this.entitiesInRange.IsNullOrEmpty())
                this.currentTarget = null;
            else
            {
                if (!this.currentTarget || !this.entitiesInRange.Contains(this.currentTarget))
                {
                    this.lastTarget = this.currentTarget;
                    this.currentTarget = closest;
                    MarkTarget();
                }
            }
        }

        private void MarkTarget()
        {
            if (!this.targetMarkerPrefab)
                return;

            if (!this.targetMarker)
                this.targetMarker = Instantiate(this.targetMarkerPrefab);

            this.targetMarker.SetActive(true);
            this.targetMarker.transform.SetParent(this.currentTarget.transform);
            this.targetMarker.transform.localPosition = Vector3.up * 4;
        }

        public void Toggle()
        {
            this.enabled = !this.enabled;
        }
    }
}