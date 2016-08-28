namespace Finsternis
{
    using UnityEngine;
    using MovementEffects;
    using System;
    using System.Collections.Generic;

    public class MenuButtonController : MonoBehaviour
    {
        [SerializeField]
        private Vector3 unselectedScale = Vector3.one/2;
        [SerializeField][Range(0.01f, 1f)]
        private float scaleInterpolationFactor = 0.2f;

        private Vector3 targetScale;
        private bool updatingScale;

        public void Select()
        {
            targetScale = Vector3.one;
            if (!updatingScale)
                Timing.RunCoroutine(_UpdateScale());
        }

        public void Deselect()
        {
            targetScale = unselectedScale;
            if (!updatingScale)
                Timing.RunCoroutine(_UpdateScale());
        }

        private IEnumerator<float> _UpdateScale()
        {
            this.updatingScale = true;
            while (transform.localScale != this.targetScale)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, this.targetScale, this.scaleInterpolationFactor);
                yield return 0;
            }
            this.updatingScale = false;
        }
    }
}