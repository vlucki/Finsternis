namespace Finsternis
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityQuery;

    [RequireComponent(typeof(CanvasGroup), typeof(Animator))]
    public abstract class FadeTransition : Transition
    {
        protected CanvasGroup canvasGroup;

        private Animator animator;

        [SerializeField][Range(0.1f, 10f)]
        private float duration = 1f;

        protected string transitionType;

        [SerializeField]
        private bool presetAlpha = false;

        protected override void Awake()
        {
            this.canvasGroup = GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();
            animator.enabled = false;

            if (!OnTransitionStarted)
                OnTransitionStarted = new TransitionEvent();

            OnTransitionStarted.AddListener(
                t =>
                {
                    animator.enabled = true;
                    animator.SetTrigger(transitionType);
                    animator.speed = 1 / duration;
                    StartCoroutine(WaitBeforEnding(duration));
                }
                );

            base.Awake();
        }

        private IEnumerator WaitBeforEnding(float duration)
        {
            yield return Yields.Seconds(duration);
            End();
        }
    }
}
