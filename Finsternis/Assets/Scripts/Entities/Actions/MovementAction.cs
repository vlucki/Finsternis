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
        private float velocityScale = 3f;

        [SerializeField]
        [Range(1, 10)]
        private float maxVelocityMagnitude = 5;

        [Range(1, 10)]
        [SerializeField]
        private float turningSpeed = 2;

        private EntityAttribute cachedSpeed;

        private EntityAttribute Speed
        {
            get { return cachedSpeed ?? (cachedSpeed = agent.GetAttribute("spd", true)); }
        }

        public float MaxVelocityMagnitude
        {
            get { return this.maxVelocityMagnitude; }
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
            if(!LastDirection.IsZero() && transform.forward != LastDirection)
            {
                UpdateRotation();
            }
        }


        private void UpdateRotation()
        {
            float currentVelocity = GetVelocityMagnitude();
            if (currentVelocity <= 0.1f)
                return;

            Quaternion rot = transform.rotation;
            rot.SetLookRotation(this.LastDirection, transform.up);
            float angle = (transform.forward.Angle(this.LastDirection));

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rot,
                Time.deltaTime * this.turningSpeed);
        }

        void FixedUpdate()
        {
            if (!this.direction.IsZero())
            {
                float velMagnitude = GetVelocityMagnitude();

                if (velMagnitude <= maxVelocityMagnitude)
                {
                    rbody.AddForce((Direction * Speed.Value) * velocityScale, ForceMode.VelocityChange);
                }
                this.LastDirection = this.direction;
                this.direction = Vector3.zero;
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