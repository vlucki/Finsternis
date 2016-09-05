namespace Finsternis
{
    using UnityEngine;

    [RequireComponent(typeof(Entity))]
    public abstract class EntityAction : MonoBehaviour
    {
        protected Entity agent;

        public Entity Agent { get { return agent; } }

        protected virtual void Awake()
        {
            agent = GetComponent<Entity>();
        }
    }
}