namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using UnityQuery;

    public class Exit : Trigger
    {
        [Serializable]
        public class ExitCrossedEvent : CustomEvent<Exit> { }

        private bool locked;

        private bool triggered;

        public ExitCrossedEvent onExitCrossed;
        private Follow camFollow;
        private GameObject mainCamera;

        public bool Locked { get { return this.locked; } }

        public bool Triggered { get { return this.triggered; } }

        protected override void Awake()
        {
            base.Awake();
            this.mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            this.camFollow = this.mainCamera.transform.parent.GetComponent<Follow>();
            this.locked = true;
            this.onExit.AddListener(OnExit);
            this.triggered = false;
        }

        public void Unlock()
        {
            if (!collider.enabled)
            {
                collider.enabled = true;
                this.camFollow.SetTarget(transform, false);
                this.camFollow.OnTargetReached.AddListener(BeginOpen);
            }
        }

        private void BeginOpen()
        {
            GetComponent<Animator>().SetTrigger("Open");
        }

        public void Open()
        {
            this.camFollow.OnTargetReached.RemoveListener(BeginOpen);
            this.camFollow.ResetTarget();
        }

        protected override void OnTriggerExit(Collider other)
        {
            if (this.triggered)
                return;
            base.OnTriggerExit(other);
        }

        protected void OnExit(GameObject other)
        {
            if (other.transform.position.y < transform.position.y)
            {
                this.triggered = true;
                collider.enabled = false;
                if (onExitCrossed)
                    onExitCrossed.Invoke(this);
            }
        }
    }
}