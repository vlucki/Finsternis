using UnityEngine;

namespace Finsternis
{
    [RequireComponent(typeof(Collider))]
    public class Blink : Skill
    {
        [SerializeField]
        private float _maxBlinkDistance = 1f;

        [SerializeField]
        private Vector3 _offset = Vector3.up;

        private Collider _collider;

        public LayerMask blockingLayers;

        protected override void Awake()
        {
            _collider = GetComponent<Collider>();
            base.Awake();
        }

        protected override void Use(int _slot)
        {
            if (MayUse(_slot))
            {
                base.Use(_slot);
                GetComponent<Animator>().SetFloat("attackSpeed", 1f);
            }
        }

        protected override void CastSkill()
        {
            float dist = _maxBlinkDistance;
            RaycastHit info;
            Vector3 origin = transform.position + _offset;
            Vector3 direction = new Vector3(Mathf.Ceil(Input.GetAxis("Horizontal")), transform.position.y, Mathf.Ceil(Input.GetAxis("Vertical")));

            if (_collider is CapsuleCollider)
            {
                CapsuleCollider cc = _collider as CapsuleCollider;
                if (Physics.CapsuleCast(cc.bounds.center + transform.up * cc.radius, cc.bounds.center - transform.up * cc.radius, cc.radius, direction, out info, dist, blockingLayers))
                    dist = info.distance - cc.radius;
            }
            else if (_collider is SphereCollider)
            {
                SphereCollider sc = _collider as SphereCollider;
                if (Physics.SphereCast(origin, sc.radius, direction, out info, dist, blockingLayers))
                    dist = info.distance - sc.radius;
            }
            else if (_collider is BoxCollider)
            {
                BoxCollider bc = _collider as BoxCollider;
                if (Physics.BoxCast(bc.center, bc.size / 2, direction, out info, Quaternion.identity, dist, blockingLayers))
                    dist = info.distance - bc.size.magnitude / 2;
            }
            if (Physics.Raycast(transform.position + direction * dist, Vector3.down, out info))
                transform.position += direction * dist;
        }
    }
}