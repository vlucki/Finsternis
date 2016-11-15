namespace Finsternis {
    using UnityEngine;
    using System.Collections.Generic;
    using System.Collections;
    using UnityQuery;

    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Follow follow;

        [SerializeField]
        [Range(1, 100)]
        private float shakeDamping = 4;

        [SerializeField]
        [Range(1, 20)]
        private float shakeFrequency = 20;

        [SerializeField]
        [Range(1, 100)]
        private float shakeAmplitude = 20;

        [SerializeField][Range(1, 100)]
        private float maxDistanceForOcclusion = 5f;

        private bool shaking;
        
        private Vector3 lastTarget;
        private Coroutine shakeHandle;

        [SerializeField]
        private bool reactToOcclusion = true;

        private int wallLayer;

        public bool ReactToOcclusion {
            get {return this.reactToOcclusion; }
            set { this.reactToOcclusion = value; }
        }

        void Awake()
        {
            if (!this.follow)
                this.follow = GetComponentInParent<Follow>();
            shaking = false;
            if (GameManager.Instance)
                GameManager.Instance.SubscribeToEvent("ShakeCamera", this, OnCameraShake);


            this.wallLayer = 1 << LayerMask.NameToLayer("Wall");
        }

        void FixedUpdate()
        {
            if (!this.follow)
                return;
            if (!this.follow.Target)
                return;

            RaycastHit hit;
            bool occlusionHappened = false;
            if (reactToOcclusion && WouldBeOccluded(out hit))
            {
                float distanceDampening = 1f - hit.distance / maxDistanceForOcclusion;
                this.follow.MemorizeOffset(this.follow.OriginalOffset + (Vector3.up * 2.5f + Vector3.forward * 4f) * distanceDampening);
                occlusionHappened = true;
            }

            if (!this.shaking)
                this.follow.ResetOffset(!occlusionHappened);

        }

        private bool WouldBeOccluded(out RaycastHit hit)
        {
            Vector3 origin = this.follow.Target.position;
            
            Ray ray = new Ray(origin + Vector3.up / 2, origin.Towards(origin+this.follow.OriginalOffset));
            float radius = 0.25f;

            return (Physics.SphereCast(ray, radius, out hit, maxDistanceForOcclusion, wallLayer)) ;
        }

        private void FinishedInterpolating()
        {
            this.follow.TranslationInterpolation = 0.1f;
            this.follow.MemorizeOffset(this.follow.OriginalOffset);
            this.follow.OnTargetReached.RemoveListener(FinishedInterpolating);
        }

        private void OnCameraShake(params object[] parameters)
        {
            Vector3? explosionPoint = null;
            if(parameters.Length > 0)
            {
                explosionPoint = parameters[0] as Vector3?;
            }
            if (explosionPoint == null)
                return;

            Vector3 relativePosition = transform.position;
            if (GameManager.Instance.Player)
                relativePosition = GameManager.Instance.Player.transform.position;

            float dist = Vector3.Distance(
                new Vector3(relativePosition.x, ((Vector3)explosionPoint).y, relativePosition.z),
                ((Vector3)explosionPoint));
            if (dist <= 0)
                dist = 1;
            if(parameters.Length > 2)
                Shake(
                    (float)parameters[2], 
                    (float)parameters[3], 
                    (float)parameters[4] / dist, 
                    (float)parameters[5],
                    (bool)parameters[1]);
            else
               Shake(0.75f, 4, 20 / dist, 20);
        }

        internal void Shake(float time, float damping, float amplitude, float frequency, bool overrideShake = true)
        {
            if (overrideShake && this.shaking)
            {
                this.shaking = false;
                StopCoroutine(this.shakeHandle);
            }

            if (!this.shaking)
            {
                this.shakeDamping = damping;
                this.shakeFrequency = frequency;
                this.shakeAmplitude = amplitude;
                this.shakeHandle = StartCoroutine(_Shake(time));
            }
        }

        IEnumerator _Shake(float shakeTime)
        {
            this.shaking = true;
            float amplitude = this.shakeAmplitude;
            while (shakeTime > 0)
            {
                float waitTime = 1 / this.shakeFrequency;
                yield return Wait.Sec(waitTime);
                shakeTime -= Time.deltaTime + waitTime;

                Vector3 shakeOffset = Random.insideUnitSphere / 10;

                shakeOffset.z = 0;
                transform.localPosition = shakeOffset * amplitude;
                transform.localRotation = Quaternion.Euler(new Vector3(Random.value, Random.value, Random.value) * amplitude / 5);
                amplitude /= this.shakeDamping;
            }

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            this.shaking = false;
        }
    }
}