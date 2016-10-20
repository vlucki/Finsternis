namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;
    using System;

    public class DynamicMessageDisplayer : MessageDisplayer
    {
        [Serializable]
        public struct ForceDirection
        {
            public Vector3 min;
            public Vector3 max;

            public ForceDirection(Vector3 min, Vector3 max)
            {
                this.min = min;
                this.max = max;
            }
        }

        [SerializeField]
        private ForceDirection forceDirection;

        void Awake()
        {
            this.forceDirection.min = this.forceDirection.min.normalized;
            this.forceDirection.max = this.forceDirection.max.normalized;
        }

        protected override MessageController DisplayMessage()
        {
            Vector3 force = forceDirection.min;
            if(this.forceDirection.min != this.forceDirection.max)
            {
                var max = this.forceDirection.max;
                force.x = UnityEngine.Random.Range(force.x, max.x);
                force.y = UnityEngine.Random.Range(force.y, max.y);
                force.z = UnityEngine.Random.Range(force.z, max.z);
            }
            
            return MessagesManager.Instance.ShowDynamicMessage(
                transform.position.WithY(1), 
                this.messageToDisplay,
                force,
                this.messageGraphic,
                this.duration);
        }


        void OnValidate()
        {
            this.forceDirection.max = Vector3.Max(this.forceDirection.max, this.forceDirection.min);
        }

    }
}