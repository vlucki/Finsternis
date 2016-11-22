namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;
    using System;
    using System.Collections;

    public class DynamicMessageDisplayer : MessageDisplayer
    {
        [Serializable]
        public enum Mode { RANDOM = 1, CONSTANT = 2, PROGRESSIVE = 3 }
        [Serializable]
        public struct ForceDirection
        {
            public Vector3 min;
            public Vector3 max;
            public Mode mode;

            public ForceDirection(Vector3 min, Vector3 max, Mode mode)
            {
                this.min = min;
                this.max = max;
                this.mode = mode;
            }
        }

        [SerializeField]
        private ForceDirection forceDirection = new ForceDirection(-Vector3.one, Vector3.one, Mode.RANDOM);

        protected override void Awake()
        {
            base.Awake();
            this.forceDirection.max = Vector3.Max(this.forceDirection.max, this.forceDirection.min);
        }

        protected override MessageController DisplayMessage()
        {
            Vector3 force = forceDirection.min;
            if (this.forceDirection.min != this.forceDirection.max && this.forceDirection.mode != Mode.PROGRESSIVE)
            {
                var max = this.forceDirection.max;
                force.x = UnityEngine.Random.Range(force.x, max.x);
                force.y = UnityEngine.Random.Range(force.y, max.y);
                force.z = UnityEngine.Random.Range(force.z, max.z);
            }

            var msg = MessagesManager.Instance.ShowDynamicMessage(
                transform.position + this.MessageOffset,
                this.messageText,
                force,
                this.messageGraphic,
                this.duration);

            if (this.forceDirection.mode != Mode.RANDOM)
            {
                StartCoroutine(_Update(force, msg));
            }

            return msg;
        }


        private IEnumerator _Update(Vector3 force, MessageController msg)
        {
            var body = msg.GetComponent<Rigidbody>();
            while (msg && msg.isActiveAndEnabled)
            {
                yield return Wait.Fixed();
                body.AddForce(force, ForceMode.Impulse);
                if (this.forceDirection.mode == Mode.PROGRESSIVE && force != this.forceDirection.max)
                {
                    force = Vector3.Lerp(force, this.forceDirection.max, Time.fixedDeltaTime);
                }
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
                Awake();
        }
#endif
    }
}