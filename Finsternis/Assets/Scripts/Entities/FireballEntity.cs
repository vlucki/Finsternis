namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityQuery;

    [RequireComponent(typeof(Animator), typeof(MovementAction), typeof(ShakeCameraEvent))]
    public class FireballEntity : Entity
    {
        [SerializeField]
        private float activeTime = 1;
        
        public UnityEvent OnTimeUp;
        public UnityEvent OnShoot;

        private MovementAction movement;
        private Animator animator;
        private ShakeCameraEvent explosionEvent;
        private readonly static int explosionTrigger;

        static FireballEntity()
        {
            explosionTrigger = Animator.StringToHash("Explode");
        }

        void Awake()
        {
            this.movement = GetComponent<MovementAction>();
            this.animator = GetComponent<Animator>();
            this.explosionEvent = GetComponent<ShakeCameraEvent>();
        }

        protected override void InitializeAttribute(int attributeIndex)
        {
            base.InitializeAttribute(attributeIndex);
            if (attributes[attributeIndex].Alias.Equals("spd"))
            {
                attributes[attributeIndex].SetValue(Random.Range(5, 9));
            }
        }

        public void Explode()
        {
            this.animator.SetTrigger(explosionTrigger);
            this.StopAllCoroutines();
            this.movement.enabled = false;
            this.movement.Rbody.isKinematic = true;
            this.explosionEvent.TriggerEvent();
        }

        public void Shoot()
        {
            if (activeTime > 0)
            {
                this.CallDelayed(this.activeTime, OnTimeUp.Invoke);
            }
            OnShoot.Invoke();
            OnShoot.RemoveAllListeners();
            movement.Rbody.isKinematic = false;
            movement.enabled = true;
        }

        void OnValidate()
        {
            this.activeTime = Mathf.Max(this.activeTime, 0);
        }
    }
}