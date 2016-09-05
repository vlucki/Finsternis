namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using System.Collections;
    using UnityQuery;

    public abstract class Transition : MonoBehaviour
    {
        [System.Serializable]
        public class TransitionEvent : UnityEvent<Transition>
        {
            public static implicit operator bool(TransitionEvent evt)
            { return evt != null; }
        }

        public bool skippable = true;

        public bool beginOnAwake = false;

        [Range(0, 5)]
        public float waitBeforeStart = 0f;
        [Range(0, 5)]
        public float waitBeforeEnding = 0f;

        public TransitionEvent OnTransitionStarted;
        public TransitionEvent OnTransitionEnded;

        private bool transitioning;

        public bool Transitioning { get { return this.transitioning; } }

        protected virtual void Awake()
        {
            if (beginOnAwake)
                Begin();
        }

        public void Begin()
        {
            StartCoroutine(_Begin());
        }

        public void End()
        {
            StartCoroutine(_End());
        }

        private IEnumerator _Begin()
        {
            if (!this.transitioning)
            {
                if (waitBeforeStart > 0)
                    yield return Yields.Seconds(waitBeforeStart);
                this.transitioning = true;
                OnTransitionStarted.Invoke(this);
            }
        }

        private IEnumerator _End()
        {
            if (this.transitioning)
            {
                this.transitioning = false;
                if (waitBeforeEnding > 0)
                    yield return Yields.Seconds(waitBeforeEnding);
                OnTransitionEnded.Invoke(this);
            }
        }

        public void Skip()
        {
            if (skippable)
            {
                this.transitioning = false;
                OnTransitionEnded.Invoke(this);
            }
        }

    }
}
