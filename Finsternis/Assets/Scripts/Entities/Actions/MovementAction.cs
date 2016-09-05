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

        void FixedUpdate()
        {
            if (!this.direction.IsZero())
            {
                float velMagnitude = GetVelocityMagnitude();

                rbody.AddForce((Direction * Speed.Value) * velocityScale, ForceMode.VelocityChange);

                if (velMagnitude > maxVelocityMagnitude)
                    rbody.AddForce(-Direction * Speed.Value * velocityScale, ForceMode.Acceleration);

                LastDirection = this.direction;
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