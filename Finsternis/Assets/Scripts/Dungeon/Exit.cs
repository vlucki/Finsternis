namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    public class Exit : Trigger
    {
        [Serializable]
        public class ExitCrossedEvent : UnityEvent<Exit>
        {
            public static implicit operator bool(ExitCrossedEvent evt)
            {
                return evt != null;
            }
        }

        [SerializeField]
        private GameObject player;

        [SerializeField]
        private GameObject cameraHolder;

        private bool locked;

        private bool triggered;

        public ExitCrossedEvent onExitCrossed;

        public bool Locked { get { return this.locked; } }

        public bool Triggered { get { return this.triggered; } }

        protected override void Awake()
        {
            base.Awake();
            if (!onExitCrossed)
                onExitCrossed = new ExitCrossedEvent();
            onExitCrossed.AddListener(GameManager.Instance.EndCurrentLevel);
            this.player = GameObject.FindGameObjectWithTag("Player");
            this.cameraHolder = GameObject.FindGameObjectWithTag("MainCamera").transform.parent.gameObject;
            this.locked = true;

            this.triggered = false;
        }

        public void Unlock()
        {
            if (!collider.enabled)
            {
                collider.enabled = true;
                Follow camFollow = this.cameraHolder.GetComponent<Follow>();
                camFollow.SetTarget(transform);
                camFollow.OnTargetReached.AddListener(BeginOpen);
            }
        }

        private void BeginOpen()
        {
            GetComponent<Animator>().SetTrigger("Open");
        }

        public void Open()
        {
            Follow camFollow = this.cameraHolder.GetComponent<Follow>();
            camFollow.OnTargetReached.RemoveListener(BeginOpen);
            camFollow.SetTarget(this.player.transform);
        }

        protected override void OnTriggerExit(Collider other)
        {
            if (this.triggered)
                return;

            base.OnTriggerExit(other);
            if (ObjectExited == this.player)
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
}