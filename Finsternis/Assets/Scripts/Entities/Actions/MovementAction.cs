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
        [Range(1f, 20f)]
        [Tooltip("The base velocity for a character")]
        private float baseVelocityMultiplier = 100f;

        [SerializeField]
        [Range(0f, 10f)]
        [Tooltip("The max velocity granted by Speed attribute")]
        private float speedMultiplier = 10f;

        [Range(1, 10)]
        [SerializeField]
        private float turningSpeed = 2;

        private EntityAttribute cachedSpeed;

        private EntityAttribute Speed
        {
            get { return cachedSpeed ?? (cachedSpeed = agent.GetAttribute("spd", true)); }
        }

        public Vector3 Velocity { get { return rbody.velocity; } }

        private Vector3 direction;

        public Vector3 Direction
        {
            get { return this.direction; }
            set { this.direction = value.normalized; }
        }

        public Vector3 LastDirection { get; private set; }

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
            {
                var forceToAdd = Direction * (this.baseVelocityMultiplier + this.speedMultiplier * this.Speed.Value / this.Speed.Max);
                rbody.AddForce(forceToAdd, ForceMode.Acceleration);

                this.LastDirection = this.direction;
            }
            else
            {
                Vector3 rbodyVel = rbody.velocity.WithY(0);
                if (!rbodyVel.IsZero())
                {
                    rbody.AddForce(-rbodyVel * baseVelocityMultiplier, ForceMode.Acceleration);
                }
            }
        }

        internal float GetVelocityMagnitude(bool ignoreY = true)
        {
            if (rbody)
            {
                if (ignoreY)
                    return rbody.velocity.XZ().sqrMagnitude;
                else
                    return rbody.velocity.sqrMagnitude;
            }
            return 0;
        }
    }
}