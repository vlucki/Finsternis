namespace Finsternis
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityQuery;

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class FadeTransition : AnimationTransition
    {
        protected CanvasGroup canvasGroup;

        public enum FadeType { FadeIn = 0, FadeOut = 1}

        protected FadeType transitionType;

        [SerializeField]
        private bool presetAlpha = false;

        protected override void Awake()
        {
            this.canvasGroup = GetComponent<CanvasGroup>();
            trigger = transitionType.ToString();

            if (presetAlpha)
                this.canvasGroup.alpha = (int)transitionType;

            base.Awake();
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            trigger = transitionType.ToString();
        }
#endif
    }
}
