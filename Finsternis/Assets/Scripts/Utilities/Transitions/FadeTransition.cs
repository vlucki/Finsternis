namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using MovementEffects;

    [RequireComponent(typeof(Graphic))]
    public abstract class FadeTransition : Transition
    {
        [SerializeField]
        [Range(0, 10)]
        private float fadeTime = 2;

        protected int targetAlpha = 1;

        private Graphic graphicToFade;

        protected override void Awake()
        {
            graphicToFade = GetComponent<Graphic>();

            if (!OnTransitionStarted)
                OnTransitionStarted = new TransitionEvent();

            OnTransitionStarted.AddListener(t => Timing.RunCoroutine(_DoFade()));

            graphicToFade.canvasRenderer.SetAlpha(1 - targetAlpha);

            base.Awake();
        }

        private IEnumerator<float> _DoFade()
        {
            graphicToFade.CrossFadeAlpha(targetAlpha, fadeTime, false);
            yield return Timing.WaitForSeconds(waitAfterEnding + fadeTime);

            End();
        }
    }
}
