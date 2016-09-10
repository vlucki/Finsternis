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

        [SerializeField]
        protected bool skippable = true;
        [SerializeField]
        protected bool beginOnStart = false;

        [Range(0, 5)]
        public float waitBeforeStart = 0f;
        [Range(0, 5)]
        public float waitBeforeEnding = 0f;

        public TransitionEvent OnTransitionStarted;
        public TransitionEvent OnTransitionEnded;

        private bool transitioning;

        public bool Transitioning { get { return this.transitioning; } }

        protected virtual void Start()
        {
            if (beginOnStart)
                Begin();
        }

        public void Begin()
        {
            if (isActiveAndEnabled)
                StartCoroutine(_Begin());
            else
                this.Error("Cannot start transition with innactive game object!");
        }

        public void End()
        {
            if(isActiveAndEnabled)
                StartCoroutine(_End());
        }

        private IEnumerator _Begin()
        {
            if (!this.transitioning)
            {
                if (waitBeforeStart > 0)
                    yield return Yields.SEC(waitBeforeStart);
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
                    yield return Yields.SEC(waitBeforeEnding);
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

        protected virtual void OnDisable()
        {
            this.transitioning = false;
        }

    }
}
