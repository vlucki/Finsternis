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

        public GameObject player { get { return GameManager.Instance.Player.gameObject; } }

        protected override void Awake()
        {
            base.Awake();
            if (!onExitCrossed)
                onExitCrossed = new ExitCrossedEvent();
            onExitCrossed.AddListener(GameManager.Instance.EndCurrentLevel);
            this.mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            this.camFollow = this.mainCamera.transform.parent.GetComponent<Follow>();
            this.locked = true;

            this.triggered = false;
        }

        public void Unlock()
        {
            if (!collider.enabled)
            {
                collider.enabled = true;
                this.camFollow.SetTarget(transform);
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
            this.camFollow.SetTarget(this.player.transform);
        }

        protected override void OnTriggerExit(Collider other)
        {
            if (this.triggered)
                return;

            base.OnTriggerExit(other);
            if (other.gameObject == this.player)
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