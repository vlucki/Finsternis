namespace Finsternis {
    using UnityEngine;
    using UnityEngine.Events;
    using Extensions;

    /// <summary>
    /// Makes game object follow a given transform.
    /// </summary>
    public class Follow : MonoBehaviour
    {

        [System.Serializable]
        public struct AxesRotationLock
        {
            public bool x;
            public bool y;
            public bool z;

            public bool all { get { return x && y && z; } }

            public AxesRotationLock(bool x, bool y, bool z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public AxesRotationLock(bool xyz) : this(xyz, xyz, xyz)
            {
            }
        }

        [SerializeField]
        private Transform target;

        [SerializeField]
        private Transform originalTarget;

        [SerializeField][Range(.01f, 10f)]
        private float tempTargetFocusTime = 0.3f;

        public UnityEvent onTargetChanged;
        public UnityEvent OnTargetReached;

        [SerializeField]
        private AxesRotationLock LockedRotationAxes = new AxesRotationLock(false);

        [SerializeField]
        private Vector3 offset = Vector3.back + Vector3.up;

        private Vector3 originalOffset;

        [SerializeField]
        [Range(0, 1)]
        private float translationInterpolation = 0.1f;

        [SerializeField]
        [Range(0, 1)]
        private float rotationInterpolation = 0.2f;

        [SerializeField]
        [Range(0,1)]
        private float distanceThreshold = 0.025f;

        private bool justReachedTarged = false;
        private Vector3 memorizedOffset;
        private Coroutine targetResetCoroutine;

        public Vector3 MemorizedOffset { get { return this.memorizedOffset; } }
        public Vector3 OriginalOffset { get { return this.originalOffset; } }

        public bool WasOffsetChanged { get { return offset == originalOffset; } }

        public Transform Target { get { return this.target; } }

        public float TranslationInterpolation { get { return this.translationInterpolation; } set { this.translationInterpolation = Mathf.Clamp01(value); } }

        void Awake()
        {
            originalOffset = offset;
            memorizedOffset = offset;
            originalTarget = target;
            if(GameManager.Instance)
                GameManager.Instance.onPlayerSpawned.AddListener(Init);
        }

        void OnDestroy()
        {
            if (GameManager.Instance)
                GameManager.Instance.onPlayerSpawned.RemoveListener(Init);
        }

        private void Init(CharController player)
        {
            SetTarget(player.transform);
        }

        public void SetTarget(Transform target)
        {
            SetTarget(target, true);
        }

        public void SetTarget(Transform target, bool setAsOriginal)
        {
            if(setAsOriginal) this.originalTarget = target;
            this.target = target;
            onTargetChanged.Invoke();
        }

        public void SetTemporaryTarget(Transform target)
        {
            SetTarget(target, false);
            if(this.targetResetCoroutine != null)
                this.StopCoroutine(this.targetResetCoroutine);
            this.targetResetCoroutine = this.CallDelayed(this.tempTargetFocusTime, ResetTarget);
        }

        public void ResetTarget()
        {
            this.target = this.originalTarget;
        }

        public void MemorizeOffset(Vector3? offset = null)
        {
            if (offset == null)
                offset = this.offset;
            this.memorizedOffset = (Vector3)offset;
        }

        public void SetOffsetX(float x)
        {
            MemorizeOffset(this.offset.Set(x: x));
        }
        public void SetOffsetY(float y)
        {
            MemorizeOffset(this.offset.Set(y: y));
        }
        public void SetOffsetZ(float z)
        {
            MemorizeOffset(this.offset.Set(z: z));
        }

        public void ResetOffset(bool toOriginal = false)
        {
            if (toOriginal)
                this.offset = this.originalOffset;
            else
                this.offset = this.memorizedOffset;
        }

        public bool FocusingTemporaryTarget()
        {
            return this.originalTarget != this.target;
        }

        void FixedUpdate()
        {
            if (!this.target)
                return;

            MoveToTarget();
            LookAtTarget();
        }

        private void MoveToTarget()
        {
            Vector3 idealPosition = this.target.position + offset;
            if (transform.position != idealPosition)
            {
                float distance = idealPosition.Distance(transform.position);

                if (distance <= this.distanceThreshold)
                {
                    if (!this.justReachedTarged)
                    {
                        this.justReachedTarged = true;
                        transform.position = idealPosition;
                        OnTargetReached.Invoke();
                    }
                }
                else
                {
                    this.justReachedTarged = false;
                    transform.position = Vector3.Slerp(transform.position, idealPosition, translationInterpolation);
                }
            }
        }

        private void LookAtTarget()
        {
            if (!LockedRotationAxes.all)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                Vector3 rotation = transform.eulerAngles;
                transform.forward = Vector3.Slerp(transform.forward, direction, this.rotationInterpolation);
                Vector3 newRotation = transform.eulerAngles;
                if (LockedRotationAxes.x)
                    newRotation.x = rotation.x;
                if (LockedRotationAxes.y)
                    newRotation.y = rotation.y;
                if (LockedRotationAxes.z)
                    newRotation.z = rotation.z;
                transform.eulerAngles = newRotation;
            }
        }
    }
}