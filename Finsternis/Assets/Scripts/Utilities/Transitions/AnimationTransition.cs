namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityQuery;

    [RequireComponent(typeof(Animator))]
    public class AnimationTransition : Transition
    {
        private Animator animator;

        protected string trigger;

        [SerializeField]
        [Range(0.1f, 10f)]
        private float duration = 1f;

        protected virtual void Awake()
        {
            this.animator = GetComponent<Animator>();
            this.animator.enabled = false;

            if (!OnTransitionStarted)
                OnTransitionStarted = new TransitionEvent();
            OnTransitionStarted.AddListener(
                t =>
                {
                    if(!animator.enabled)
                        animator.enabled = true;
                    animator.speed = 1 / duration;
                    if(!trigger.IsNullOrEmpty())
                        animator.SetTrigger(trigger);
                    StartCoroutine(WaitForAnimation(duration));
                }
            );
        }

        private IEnumerator WaitForAnimation(float duration)
        {
            float elapsed = 0;
            while(Transitioning && elapsed < duration)
            {
                yield return Yields.Frame();
                elapsed += Time.deltaTime;
            }

            if (elapsed < duration)
                this.animator.ResetTrigger(trigger);

            if(Transitioning)
                End();
        }
    }
}