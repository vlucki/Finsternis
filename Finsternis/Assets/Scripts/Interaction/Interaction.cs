namespace Finsternis {
    using UnityEngine;
    using System.Collections;

    [RequireComponent(typeof(InteractionModule))]
    public abstract class Interaction : CustomBehaviour, IInteractable
    {
        protected Entity entity;

        protected virtual void Awake()
        {
            this.entity = GetComponent<Entity>();
        }

        public virtual bool Interact(InteractionData data)
        {
            return data.InteractionType.Equals(this.GetType());
        }
    }
}