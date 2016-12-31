namespace Finsternis
{
    using UnityEngine;

    [RequireComponent(typeof(Entity))]
    public abstract class InteractionModule : MonoBehaviour
    {
        public Entity LastInteractedWith { get; private set; }

        [System.Serializable]
        public class InteractionEvent : CustomEvent<InteractionData> { }

        public void Interact(InteractionData data)
        {
            foreach(var i in GetComponents<Interaction>())
            {
                if (i.Interact(data))
                {
                    LastInteractedWith = data.Entity;
                }
            };
        }
    }
}