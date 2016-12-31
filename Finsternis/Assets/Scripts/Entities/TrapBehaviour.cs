namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(Animator))]
    public class TrapBehaviour : Entity
    {
        public UnityEvent onDeactivated;

        void Deactivate()
        {
            onDeactivated.Invoke();
        }

    }
}