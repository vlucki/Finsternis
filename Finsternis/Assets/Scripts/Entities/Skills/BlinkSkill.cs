using System;
using UnityEngine;
using UnityQuery;

namespace Finsternis
{
    [RequireComponent(typeof(Collider))]
    public class BlinkSkill : Skill
    {
        [Space(10)]
        [Header("Blink attributes", order = 1)]

        [SerializeField]
        [Range(0, 10)]
        private float minBlinkDistance = 0.5f;

        [SerializeField][Range(1, 10)]
        private float maxBlinkDistance = 3f;

        [SerializeField]
        private Vector3 offset = Vector3.up;

        [SerializeField]
        private LayerMask blockingLayers;

        private new Collider collider;

        protected override void Awake()
        {
            this.collider = GetComponent<Collider>();
            base.Awake();
        }

        public override void StartExecution()
        {
            Vector3 origin = transform.position + this.offset;
            Vector3 direction = GetBlinkDirection();

            float maxValidDistance = maxBlinkDistance;
            RaycastHit hit;
            if(Physics.Raycast(new Ray(origin, direction), out hit, maxValidDistance, blockingLayers))
                maxValidDistance = hit.distance;

            if (maxValidDistance >= this.minBlinkDistance)
            {
                Vector3 destination;

                if (GetValidDestination(maxValidDistance, origin, direction, out destination))
                {
                    transform.position = destination;
                    base.StartExecution();
                }
                else
                {
                    user.Controller.SetTrigger("stopAttack");
                    End();
                }
            }
        }

        private bool GetValidDestination(float maxValidDistance, Vector3 origin, Vector3 direction, out Vector3 intendedDestination)
        {
            int floorLayer = (1 << LayerMask.NameToLayer("Floor"));
            int layersThatCantIntersect = (1 << LayerMask.NameToLayer("Wall"));

            intendedDestination = origin + direction * maxValidDistance;

            while (maxValidDistance >= this.minBlinkDistance)
            {                
                var ray = new Ray(intendedDestination, Vector3.down);

                if (Physics.Raycast(ray, 2f, floorLayer)) //if there's a floor tile below the future position of this entity
                {
                    var overlap = Physics.OverlapSphere(intendedDestination, GetColliderRadius(), blockingLayers | layersThatCantIntersect);
                    //and said position can fit the entity
                    if (overlap.Length == 0)
                        break; //no need to keep on checking
                }
                maxValidDistance -= 0.1f;
                intendedDestination = origin + direction * maxValidDistance;
            }

            return maxValidDistance >= this.minBlinkDistance;
        }

        private float GetColliderRadius()
        {
            if(this.collider is CapsuleCollider)
            {
                return this.collider.bounds.extents.WithY(0).sqrMagnitude;
            }

            return 0.1f;
        }

        private Vector3 GetBlinkDirection()
        {
            Vector3 blinkDirection = this.user.Movement.LastMovementDirection;

            if (blinkDirection.IsZero())
                blinkDirection = transform.forward;

            return blinkDirection;
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            this.maxBlinkDistance = Mathf.Max(this.maxBlinkDistance, this.minBlinkDistance);
        }
#endif
    }
}