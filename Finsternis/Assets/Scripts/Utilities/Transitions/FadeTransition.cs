namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using MovementEffects;
    
    public abstract class FadeTransition : Transition
    {
        [SerializeField]
        [Range(0, 10)]
        private float fadeTime = 2;

        protected int targetAlpha = 1;

        private List<Graphic> graphicsToFade;

        [SerializeField]
        private bool setupAlphaOnAwake = true;

        protected override void Awake()
        {
            this.graphicsToFade = new List<Graphic>();
            GetComponentsInChildren<Graphic>(graphicsToFade);

            if (!OnTransitionStarted)
                OnTransitionStarted = new TransitionEvent();

            OnTransitionStarted.AddListener(t => Timing.RunCoroutine(_DoFade()));

            if(setupAlphaOnAwake)
                foreach(var toFade in this.graphicsToFade)
                    toFade.canvasRenderer.SetAlpha(1 - targetAlpha);

            base.Awake();
        }

        private IEnumerator<float> _DoFade()
        {
            foreach (var toFade in this.graphicsToFade)
                toFade.CrossFadeAlpha(targetAlpha, fadeTime, false);
            yield return Timing.WaitForSeconds(waitAfterEnding + fadeTime);

            End();
        }
    }
}
