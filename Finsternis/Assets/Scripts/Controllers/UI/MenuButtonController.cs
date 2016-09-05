namespace Finsternis
{
    using UnityEngine;

    using System;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using UnityEngine.Events;
    using System.Collections;

    public class MenuButtonController : Button
    {
        [Serializable]
        public class SelectionChangedEvent : UnityEvent<bool, Selectable>{
            public static implicit operator bool(SelectionChangedEvent evt) { return evt != null; }
        }

        public SelectionChangedEvent OnSelectionChanged;
        [SerializeField]
        private Vector3 unselectedScale = Vector3.one/2;
        [SerializeField][Range(0.01f, 1f)]
        private float transitionInterpolationFactor = 0.2f;

        private Text label;
        private Vector3 targetScale;
        private float targetLabelAlpha;

        public Button Button { get; private set; }
        public bool IsSelected { get; private set; }

        protected override void Awake()
        {
            this.label = GetComponentInChildren<Text>();
            base.Awake();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            this.IsSelected = true;
            this.targetScale = Vector3.one;
            this.targetLabelAlpha = 1;

            StopCoroutine(_DoTransition());
            StartCoroutine(_DoTransition());

            if (OnSelectionChanged)
                OnSelectionChanged.Invoke(true, this);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            this.IsSelected = false;
            this.targetScale = unselectedScale;
            this.targetLabelAlpha = 0.25f;

            StopCoroutine(_DoTransition());
            StartCoroutine(_DoTransition());

            if (OnSelectionChanged)
                OnSelectionChanged.Invoke(true, this);
        }

        private IEnumerator _DoTransition()
        {
            Color c = label.color;
            while (transform.localScale != this.targetScale)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, this.targetScale, this.transitionInterpolationFactor);
                c.a = Mathf.Lerp(c.a, this.targetLabelAlpha, this.transitionInterpolationFactor);
                label.color = c;
                yield return null;
            }
            transform.localScale = this.targetScale;
            c.a = this.targetLabelAlpha;
            label.color = c;
        }
    }
}