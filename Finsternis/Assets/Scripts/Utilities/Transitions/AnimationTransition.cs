namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityQuery;

    [RequireComponent(typeof(Animator))]
    public class AnimationTransition : Transition
    {
        private Animator animator;

        [SerializeField][ReadOnly]
        protected string trigger;

        [SerializeField]
        [Range(0.1f, 10f)]
        protected float duration = 1f;

        public float Duration { get { return this.duration; } }

        protected virtual void Awake()
        {
            this.animator = GetComponent<Animator>();
            this.animator.enabled = false;

            if (!OnTransitionStarted)
                OnTransitionStarted = new TransitionEvent();
            OnTransitionStarted.AddListener(
                t =>
                {
                    if (!animator.enabled)
                        animator.enabled = true;

                    if(!trigger.IsNullOrEmpty())
                        animator.SetFloat(trigger+"Speed", 1 / duration);

                    if (!trigger.IsNullOrEmpty())
                        animator.SetTrigger(trigger);

                    if(isActiveAndEnabled)
                        StartCoroutine(WaitForAnimation(duration));
                }
            );
        }

        private IEnumerator WaitForAnimation(float duration)
        {
            float elapsed = 0;
            while(Transitioning && elapsed <= duration)
            {
                yield return Wait.Frame();
                elapsed += Time.deltaTime;
            }

            if (elapsed < duration)
                this.animator.ResetTrigger(trigger);

            if(Transitioning)
                End();
        }

        public void SetDuration(float duration)
        {
            this.duration = Mathf.Max(0.1f, duration);
        }

        public void BeginWithTrigger(string trigger)
        {
            this.trigger = trigger;
            Begin();
        }
    }
}