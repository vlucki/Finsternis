namespace Finsternis {
    using UnityEngine;
    using System.Collections;

    [RequireComponent(typeof(InteractionModule))]
    public abstract class Interaction : CustomBehaviour
    {
        protected Entity entity;

        protected virtual void Awake()
        {
            this.entity = GetComponent<Entity>();
            GetComponent<InteractionModule>().onInteraction += Interact;
        }

        protected bool ValidadeInteractionType(InteractionData data)
        {
            return data.InteractionType.Equals(this.GetType());
        }

        protected abstract bool Interact(InteractionData data);
    }
}