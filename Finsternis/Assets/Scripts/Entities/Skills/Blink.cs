using UnityEngine;

namespace Finsternis
{
    [RequireComponent(typeof(Collider))]
    public class Blink : Skill
    {
        [Space(10)]
        [Header("Blink attributes", order = 1)]

        [SerializeField]
        private float _maxBlinkDistance = 1f;

        [SerializeField]
        private Vector3 _offset = Vector3.up;

        private new Collider collider;

        public LayerMask blockingLayers;

        private Animator animator;

        protected override void Awake()
        {
            this.collider = GetComponent<Collider>();
            this.animator = GetComponent<Animator>();
            base.Awake();
        }

        public override bool Use()
        {
            if (MayUse())
            {
                this.animator.SetFloat("attackSpeed", 1f);
            }
            return base.Use();
        }

        protected override void CastSkill()
        {
            base.CastSkill();
            float dist = _maxBlinkDistance;
            RaycastHit info;
            Vector3 origin = transform.position + _offset;
            Vector3 direction = new Vector3(Mathf.Ceil(Input.GetAxis("Horizontal")), transform.position.y, Mathf.Ceil(Input.GetAxis("Vertical")));

            if (this.collider is CapsuleCollider)
            {
                CapsuleCollider cc = collider as CapsuleCollider;
                if (Physics.CapsuleCast(cc.bounds.center + transform.up * cc.radius, cc.bounds.center - transform.up * cc.radius, cc.radius, direction, out info, dist, blockingLayers))
                    dist = info.distance - cc.radius;
            }
            else if (this.collider is SphereCollider)
            {
                SphereCollider sc = collider as SphereCollider;
                if (Physics.SphereCast(origin, sc.radius, direction, out info, dist, blockingLayers))
                    dist = info.distance - sc.radius;
            }
            else if (this.collider is BoxCollider)
            {
                BoxCollider bc = collider as BoxCollider;
                if (Physics.BoxCast(bc.center, bc.size / 2, direction, out info, Quaternion.identity, dist, blockingLayers))
                    dist = info.distance - bc.size.magnitude / 2;
            }
            if (Physics.Raycast(transform.position + direction * dist, Vector3.down, out info))
                transform.position += direction * dist;
        }
    }
}