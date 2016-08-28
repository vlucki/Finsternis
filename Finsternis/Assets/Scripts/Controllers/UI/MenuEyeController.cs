namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.UI;
    using MovementEffects;
    using System;
    using System.Collections.Generic;
    using UnityEngine.EventSystems;
    using UnityQuery;

    [RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    public class MenuEyeController : MonoBehaviour
    {
        [SerializeField]
        [Range(0.01f, 1)]
        private float interpolationAmount = 0.2f;
        
        private GameObject pupil;

        private IEnumerator<float> lookAtEnumerator;
        private Vector2 lastTarget;

        private Circle eyeBounds;

        void Awake()
        {
            var t = GetComponent<RectTransform>();
            eyeBounds = new Circle(t.sizeDelta.Min() / 2, t.anchoredPosition);
            try
            {
                pupil = transform.Find("Pupil").gameObject;
            } catch(NullReferenceException ex)
            {
                Log.Error(this, "Failed to find eye pupil.\n" + ex.Message);
            }
        }

        public void LookAt(BaseEventData data)
        {
            if (!pupil)
                return;

            Vector2 target = eyeBounds.ProjectPoint(data.selectedObject.GetComponent<RectTransform>().anchoredPosition);
            if (lastTarget == target)
                return;

            lastTarget = target;

            if (lookAtEnumerator != null)
                Timing.KillCoroutines(lookAtEnumerator);

            lookAtEnumerator = Timing.RunCoroutine(_LookAtTarget(target));
        }

        private IEnumerator<float> _LookAtTarget(Vector2 target)
        {
            Vector2 currentPos;
            Rect eyeBounds = GetComponent<RectTransform>().rect;
            var transform = pupil.GetComponent<RectTransform>();

            do
            {
                currentPos = Vector2.Lerp(transform.anchoredPosition, target, this.interpolationAmount);
                transform.anchoredPosition = currentPos;
                yield return 0;
            } while (currentPos != target);
        }
        
    }
}