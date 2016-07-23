using UnityEngine;

namespace Finsternis
{
    [RequireComponent(typeof(Collider))]
    public class Blink : Skill
    {
        [Space(10)]
        [Header("Blink attributes", order = 1)]

        [SerializeField][Range(0, 10)]
        private float maxBlinkDistance = 1f;

        [SerializeField]
        private Vector3 offset = Vector3.up;

        [SerializeField]
        private LayerMask blockingLayers;

        private new Collider collider;

        private Animator animator;

        protected override void Awake()
        {
            this.collider = GetComponent<Collider>();
            this.animator = GetComponent<Animator>();
            base.Awake();
        }

        public override void Use()
        {
            this.animator.SetFloat(CharController.AttackSpeed, 1f);
            base.Use();
        }

        protected override void CastSkill()
        {
            base.CastSkill();
            float dist = this.maxBlinkDistance;
            RaycastHit info;
            Vector3 origin = transform.position + this.offset;
            Vector3 direction = new Vector3(Mathf.Ceil(Input.GetAxis("Horizontal")), transform.position.y, Mathf.Ceil(Input.GetAxis("Vertical")));

            if (this.collider is CapsuleCollider)
            {
                CapsuleCollider cc = collider as CapsuleCollider;
                if (Physics.CapsuleCast(cc.bounds.center + transform.up * cc.radius, cc.bounds.center - transform.up * cc.radius, cc.radius, direction, out info, dist, this.blockingLayers))
                    dist = info.distance - cc.radius;
            }
            else if (this.collider is SphereCollider)
            {
                SphereCollider sc = collider as SphereCollider;
                if (Physics.SphereCast(origin, sc.radius, direction, out info, dist, this.blockingLayers))
                    dist = info.distance - sc.radius;
            }
            else if (this.collider is BoxCollider)
            {
                BoxCollider bc = collider as BoxCollider;
                if (Physics.BoxCast(bc.center, bc.size / 2, direction, out info, Quaternion.identity, dist, this.blockingLayers))
                    dist = info.distance - bc.size.magnitude / 2;
            }

            //Check if there's a floor where the character may stand at the destination
            if (Physics.Raycast(transform.position + direction * dist, Vector3.down, out info))
                transform.position += direction * dist;
        }
    }
}