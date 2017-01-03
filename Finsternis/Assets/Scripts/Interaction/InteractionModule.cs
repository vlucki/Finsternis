namespace Finsternis
{
    using UnityEngine;

    [RequireComponent(typeof(Entity)), DisallowMultipleComponent]
    public class InteractionModule : MonoBehaviour
    {
        public Entity LastInteraction { get; private set; }

        public delegate bool InteractionDelegate(InteractionData data);
        public event InteractionDelegate onInteraction;

        public void Interact(InteractionData data)
        {
            if(onInteraction(data))
                LastInteraction = data.Entity;
        }
    }
}