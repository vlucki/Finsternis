namespace Finsternis {
    using UnityEngine;
    using System.Collections;
    using UnityEngine.Events;
    using UnityQuery;
    using System;

    public class Follow : MonoBehaviour
    {
        [SerializeField]
        private bool focusTarget = true;

        [SerializeField]
        private Transform target;

        [SerializeField]
        private Transform originalTarget;

        [SerializeField]
        private float tempTargetFocusTime = 0.3f;

        private float timeFocusingTempTarget = 0;

        public UnityEvent OnTargetReached;

        [System.Serializable]
        public struct AxesRotationLock
        {
            public bool x;
            public bool y;
            public bool z;

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

        public AxesRotationLock LockedRotationAxes = new AxesRotationLock(false);

        public Vector3 offset = Vector3.back + Vector3.up;

        private Vector3 originalOffset;

        [Range(0, 1)]
        public float translationInterpolation = 0.1f;

        [Range(0, 1)]
        public float rotationInterpolation = 0.2f;

        [Range(0,1)]
        public float distanceThreshold = 0.025f;

        private Vector3 memorizedOffset;

        public Vector3 MemorizedOffset { get { return this.memorizedOffset; } }
        public Vector3 OriginalOffset { get { return this.originalOffset; } }

        public bool WasOffsetChanged { get { return offset == originalOffset; } }

        public Transform Target { get { return this.target; } }

        void Awake()
        {
            originalOffset = offset;
            memorizedOffset = offset;
            originalTarget = target;
            if(GameManager.Instance)
                GameManager.Instance.OnPlayerSpawned.AddListener(Init);
        }

        void OnDestroy()
        {
            if (GameManager.Instance)
                GameManager.Instance.OnPlayerSpawned.RemoveListener(Init);
        }

        private void Init()
        {
            SetTarget(GameManager.Instance.Player.transform);
        }

        public void SetTarget(Transform target)
        {
            this.originalTarget = target;
            this.target = target;
        }

        public void SetTemporaryTarget(Transform target)
        {
            if (this.originalTarget == null)
                this.originalTarget = this.target;
            this.target = target;
            timeFocusingTempTarget = 0;
        }

        public void MemorizeOffset(Vector3? offset = null)
        {
            if (offset == null)
                offset = this.offset;
            this.memorizedOffset = (Vector3)offset;
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

        void Update()
        {
            if (!this.target)
                return;

            Vector3 idealPosition = this.target.position + offset;
            float distance = idealPosition.Distance(transform.position);

            if (distance <= this.distanceThreshold)
            {
                transform.position = idealPosition;
                OnTargetReached.Invoke();
                if (this.tempTargetFocusTime > 0 && FocusingTemporaryTarget())
                    this.timeFocusingTempTarget += Time.deltaTime;
            }
            else
            {
                transform.position = Vector3.Slerp(transform.position, idealPosition, translationInterpolation);
            }

            if (timeFocusingTempTarget >= tempTargetFocusTime)
                target = originalTarget;

            if (focusTarget && !(LockedRotationAxes.x && LockedRotationAxes.y && LockedRotationAxes.z))
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