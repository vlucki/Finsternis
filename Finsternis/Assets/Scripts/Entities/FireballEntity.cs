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
        
        private MovementAction movement;

        void Awake()
        {
            movement = GetComponent<MovementAction>();
        }

        protected override void InitializeAttribute(int attributeIndex)
        {
            base.InitializeAttribute(attributeIndex);
            if (attributes[attributeIndex].Alias.Equals("spd"))
                attributes[attributeIndex].SetValue(10);
        }

        internal void Explode()
        {
            movement.enabled = false;
            movement.Rbody.isKinematic = true;
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            var player = GameManager.Instance.Player.transform;
            float dist = Vector3.Distance(
                new Vector3(player.position.x, transform.position.y, player.position.z),
                transform.position);
            if (dist <= 0)
                dist = 1;
            cam.GetComponent<CameraController>().Shake(0.75f, 4, 20 / dist, 20);
        }

        public void Shoot()
        {
            if (activeTime > 0)
            {
                this.CallDelayed(this.activeTime, OnTimeUp.Invoke);
            }
            movement.Rbody.isKinematic = false;
            movement.enabled = true;
        }

        void OnValidate()
        {
            this.activeTime = Mathf.Max(this.activeTime, 0);
        }
    }
}