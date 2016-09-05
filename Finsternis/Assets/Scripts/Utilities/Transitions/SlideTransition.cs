namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityQuery;
    using System.Collections.Generic;

    public abstract class SlideTransition : Transition
    {
        [SerializeField]
        private Vector2 direction;


        protected override void Awake()
        {
            base.Awake();
            OnTransitionStarted.AddListener(t => StartCoroutine(_DoSlide()));
        }

        private IEnumerator<float> _DoSlide()
        {
            yield return 0;
        }

#if UNITY_EDITOR
            void OnValidate()
        {
            direction.LinearClamp(-Vector2.one, Vector2.one);
        }
#endif

    }
}