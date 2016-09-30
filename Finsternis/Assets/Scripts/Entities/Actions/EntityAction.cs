namespace Finsternis
{
    using UnityEngine;

    [RequireComponent(typeof(Entity))]
    public abstract class EntityAction : MonoBehaviour
    {
        protected Entity agent;

        public enum ActionType
        {
            ATTACK = 0,
            OPEN = 1
        }

        public ActionType Type { get; protected set; }

        public Entity Agent { get { return agent; } }

        protected virtual void Awake()
        {
            agent = GetComponent<Entity>();
        }
    }
}