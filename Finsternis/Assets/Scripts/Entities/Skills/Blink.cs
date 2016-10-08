using System;
using UnityEngine;
using UnityQuery;

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

            Vector3 origin = transform.position + this.offset;
            Vector3 direction = GetBlinkDirection();

            float maxValidDistance = maxBlinkDistance;
            RaycastHit hit;
            if(Physics.Raycast(new Ray(origin, direction), out hit, maxValidDistance, blockingLayers))
                maxValidDistance = hit.distance;

            int floorLayer = (1 << LayerMask.NameToLayer("Floor"));
            int layersThatCantIntersect = (1 << LayerMask.NameToLayer("Wall"));
            while (maxValidDistance > 0)
            {
                var ray = new Ray(origin + direction * maxValidDistance, Vector3.down);

                if (Physics.Raycast(ray, 2f, floorLayer)) //if there's a floor tile below the future position of this entity
                {
                    var overlap = Physics.OverlapSphere(ray.origin, 0.25f, blockingLayers | layersThatCantIntersect);
                        //and sayd position can fit the entity
                        if (overlap.Length == 0)
                            break; //no need to keep on checking
                }
                maxValidDistance -= 0.1f;
            }
            if(maxValidDistance > 0)
            {
                transform.position += direction * maxValidDistance;
            }
        }

        private Vector3 GetBlinkDirection()
        {
            Vector3 blinkDirection = transform.forward;
            var movementAction = GetComponent<MovementAction>();
            if (movementAction && !movementAction.Direction.IsZero())
                blinkDirection = movementAction.Direction.normalized;

            return blinkDirection;
        }
    }
}