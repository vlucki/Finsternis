namespace Finsternis
{
    using UnityEngine;
    using MovementEffects;
    using System;
    using System.Collections.Generic;
    using UnityEngine.UI;

    public class MenuButtonController : MonoBehaviour
    {
        [SerializeField]
        private Vector3 unselectedScale = Vector3.one/2;
        [SerializeField][Range(0.01f, 1f)]
        private float transitionInterpolationFactor = 0.2f;

        private Text label;
        private Vector3 targetScale;
        private float targetLabelAlpha;
        private bool transitioning;

        void Awake()
        {
            this.label = GetComponentInChildren<Text>();
        }

        public void Select()
        {
            targetScale = Vector3.one;
            targetLabelAlpha = 1;
            if (!transitioning)
                Timing.RunCoroutine(_DoTransition());
        }

        public void Deselect()
        {
            targetScale = unselectedScale;
            targetLabelAlpha = 0.25f;
            if (!transitioning)
                Timing.RunCoroutine(_DoTransition());
        }

        private IEnumerator<float> _DoTransition()
        {
            this.transitioning = true;
            Color c = label.color;
            while (transform.localScale != this.targetScale)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, this.targetScale, this.transitionInterpolationFactor);
                c.a = Mathf.Lerp(c.a, this.targetLabelAlpha, this.transitionInterpolationFactor);
                label.color = c;
                yield return 0;
            }
            transform.localScale = this.targetScale;
            c.a = this.targetLabelAlpha;
            label.color = c;
            this.transitioning = false;
        }
    }
}