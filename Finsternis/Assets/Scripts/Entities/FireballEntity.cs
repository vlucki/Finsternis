namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityQuery;

    [RequireComponent(typeof(Animator), typeof(MovementAction))]
    public class FireballEntity : Entity
    {
        [SerializeField]
        private float activeTime = 1;
        
        public UnityEvent OnTimeUp;
        public UnityEvent OnShoot;

        private MovementAction movement;

        void Awake()
        {
            movement = GetComponent<MovementAction>();
        }

        protected override void InitializeAttribute(int attributeIndex)
        {
            base.InitializeAttribute(attributeIndex);
            if (attributes[attributeIndex].Alias.Equals("spd"))
            {
                attributes[attributeIndex].SetValue(Random.Range(5, 9));
            }
        }

        internal void Explode()
        {
            movement.enabled = false;
            movement.Rbody.isKinematic = true;
            if (GameManager.Instance)
            {
                GameManager.Instance.TriggerGlobalEvent("Explosion", transform.position);
            }
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