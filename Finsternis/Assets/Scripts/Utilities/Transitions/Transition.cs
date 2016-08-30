namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using MovementEffects;
    using System.Collections.Generic;

    public abstract class Transition : MonoBehaviour
    {
        public bool skippable = true;

        public bool beginOnAwake = false;

        [Range(0, 5)]
        public float waitBeforeStart = 0f;
        [Range(0, 5)]
        public float waitAfterEnding = 0f;

      public UnityEvent OnTransitionStarted;
        public UnityEvent OnTransitionEnded;

        private bool transitioning;

        public bool Transitioning { get { return this.transitioning; } }

        protected virtual void Awake()
        {
            if (beginOnAwake)
                Begin();
        }

        public void Begin()
        {
            Timing.RunCoroutine(_Begin());
        }

        public void End()
        {
            Timing.RunCoroutine(_End());
        }

        private IEnumerator<float> _Begin()
        {
            if (!transitioning)
            {
                if (waitBeforeStart > 0)
                    yield return Timing.WaitForSeconds(waitBeforeStart);
                this.transitioning = true;
                OnTransitionStarted.Invoke();
            }
        }

        private IEnumerator<float> _End()
        {
            if (transitioning)
            {
                if (waitBeforeStart > 0)
                    yield return Timing.WaitForSeconds(waitBeforeStart);
                this.transitioning = false;
                OnTransitionEnded.Invoke();
            }
        }

        public void Skip()
        {
            if (skippable)
            {
                End();
            }
        }

    }
}
