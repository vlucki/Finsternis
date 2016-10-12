namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using UnityQuery;

    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class MovementAction : EntityAction
    {
        private Rigidbody rbody;

        [SerializeField]
        [Range(1f, 30f)]
        [Tooltip("The base velocity for a character")]
        private float baseVelocityMultiplier = 10f;

        [SerializeField]
        [Range(0f, 30f)]
        [Tooltip("The max velocity granted by Speed attribute")]
        private float speedMultiplier = 10f;

        [Range(1, 10)]
        [SerializeField]
        private float turningSpeed = 2;

        [SerializeField]
        private ForceMode modeWhenApplyingForce = ForceMode.Acceleration;

        private EntityAttribute cachedSpeed;

        private Vector3 direction;

        private EntityAttribute Speed
        {
            get { return this.cachedSpeed ?? (this.cachedSpeed = agent.GetAttribute("spd", true)); }
        }

        public Vector3 Velocity { get { return this.rbody.velocity; } }

        public Vector3 Direction
        {
            get { return this.direction; }
            set { this.direction = value.normalized; }
        }

        public Rigidbody Rbody { get { return this.rbody; } }

        protected override void Awake()
        {
            base.Awake();
            this.rbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (!this.direction.IsZero() && transform.forward != this.direction)
                UpdateRotation();
        }

        private void UpdateRotation()
        {
            Quaternion rot = transform.rotation;
            rot.SetLookRotation(this.direction, transform.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rot,
                Time.deltaTime * this.turningSpeed);
        }

        void FixedUpdate()
        {
            if (!this.direction.IsZero())
                Move(Direction);
            else if (!this.Velocity.IsZero())
                Move(-Velocity.WithY(0).normalized);
        }

        private void Move(Vector3 direction)
        {
            if (!direction.IsZero())
                this.rbody.AddForce(
                    direction *
                    (this.baseVelocityMultiplier + this.speedMultiplier * this.Speed.Value / this.Speed.Max),
                    modeWhenApplyingForce);
        }

        internal float GetVelocityMagnitude(bool ignoreY = true)
        {
            if (this.rbody)
            {
                if (ignoreY)
                    return Velocity.WithY(0).sqrMagnitude;
                else
                    return Velocity.sqrMagnitude;
            }
            return 0;
        }
    }
}