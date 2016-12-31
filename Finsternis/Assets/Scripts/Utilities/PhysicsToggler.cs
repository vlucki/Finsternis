namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using Extensions;

    public class PhysicsToggler : MonoBehaviour
    {
        [SerializeField]
        private string referenceObjectTag;

        [SerializeField]
        [Range(1, 99)]
        private float distanceThreshold = 20;

        private bool wasInRange = true;

        private Transform transformToObserve;

        private List<Rigidbody> rigidbodies;
        private List<Collider> colliders;
        private List<Renderer> renderers;

        void Awake()
        {
            var obj = GameObject.FindGameObjectWithTag(referenceObjectTag);
            if (obj)
                this.transformToObserve = obj.transform;
            else
                this.Disable();
        }

        void Start()
        {
            this.rigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
            this.colliders = new List<Collider>(GetComponentsInChildren<Collider>());
            this.renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
            if (this.rigidbodies.IsNullOrEmpty() && this.colliders.IsNullOrEmpty() && this.renderers.IsNullOrEmpty())
                this.Disable();
            else
                Toggle();
        }

        void FixedUpdate()
        {
            if (!this.transformToObserve)
                this.Disable();
            else
            {
                Toggle();
            }
        }

        private void Toggle()
        {
            bool inRange = this.transformToObserve.position.Distance(this.transform.position) <= this.distanceThreshold;
            if (!(inRange && this.wasInRange))
            {
                this.wasInRange = inRange;
                int lastIndex = Mathf.Max(this.rigidbodies.Count, this.colliders.Count, this.renderers.Count);
                for (int i = 0; i < lastIndex; i++)
                {
                    if (i < this.rigidbodies.Count)
                        Toggle(this.rigidbodies[i], inRange);

                    if (i < this.colliders.Count)
                        Toggle(this.colliders[i], inRange);

                    if (i < this.renderers.Count)
                        Toggle(this.renderers[i], inRange);

                }
            }
        }

        private void Toggle(Rigidbody body, bool enabled)
        {
            if (!enabled)
                body.Sleep();
            body.detectCollisions = enabled;
        }

        private void Toggle(Collider col, bool enabled)
        {
            col.enabled = enabled;
            return;
        }

        private void Toggle(Renderer rend, bool enabled)
        {
            rend.enabled = enabled;
            return;
        }
    }
}