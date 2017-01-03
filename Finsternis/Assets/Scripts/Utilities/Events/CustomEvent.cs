namespace Finsternis.Events
{
    using System;
    using UnityEngine.Events;

    public class CustomEvent<T> : UnityEvent<T>
    {
        public static implicit operator bool(CustomEvent<T> evt)
        {
            return evt != null;
        }
    }

    public class CustomEvent<T1, T2> : UnityEvent<T1, T2>
    {
        public static implicit operator bool(CustomEvent<T1, T2> evt)
        {
            return evt != null;
        }
    }

    [Serializable]
    public class BehaviourEvent : CustomEvent<CustomBehaviour>
    { }
}