namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using Extensions;

    [RequireComponent(typeof(Animator), typeof(MovementAction), typeof(ShakeCameraEvent))]
    public class FireballEntity : Entity
    {
        [SerializeField]
        private float activeTime = 1;
        
        public UnityEvent onTimeUp;
        public UnityEvent onShoot;
        public UnityEvent onExplode;

        private MovementAction movement;
        private Animator animator;
        private ShakeCameraEvent explosionEvent;
        private readonly static int explosionTrigger;

        private bool exploded = false;

        static FireballEntity()
        {
            explosionTrigger = Animator.StringToHash("Explode");
        }

        protected override void Awake()
        {
            this.onAttributeInitialized.AddListener(GrabSpeed);
            base.Awake();
            this.movement = GetComponent<MovementAction>();
            this.animator = GetComponent<Animator>();
            this.explosionEvent = GetComponent<ShakeCameraEvent>();
        }

        protected void GrabSpeed(Attribute attribute)
        {
            if (attribute.Alias.Equals("spd"))
            {
                attribute.Value = (Random.Range(5, 9));
            }
        }

        public void Explode()
        {
            if (this.exploded)
                return;
            this.exploded = true;
            this.animator.SetTrigger(explosionTrigger);
            this.StopAllCoroutines();
            this.movement.enabled = false;
            this.movement.Rbody.isKinematic = true;
            onExplode.Invoke();
            this.explosionEvent.TriggerEvent();
        }

        public void Shoot()
        {
            if (activeTime > 0)
            {
                this.CallDelayed(this.activeTime, onTimeUp.Invoke);
            }
            onShoot.Invoke();
            onShoot.RemoveAllListeners();
            this.movement.Rbody.isKinematic = false;
            this.movement.enabled = true;
            this.exploded = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Explode();
        }

        void OnValidate()
        {
            this.activeTime = Mathf.Max(this.activeTime, 0);
        }
    }
}