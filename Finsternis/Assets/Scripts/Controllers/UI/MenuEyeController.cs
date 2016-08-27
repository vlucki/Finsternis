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

        [Serializable]
        public struct Circle
        {
            public readonly float radius;
            public readonly Vector2 center;

            public Circle(float radius, Vector2 center)
            {
                this.radius = radius;
                this.center = center;
            }

            public bool Contains(Vector2 point)
            {
                return Vector2.Distance(point, center) <= radius;
            }

            public Vector2 ProjectPoint(Vector2 point)
            {
                return  Vector2.MoveTowards(this.center, point, this.radius);
            }
        }

        [SerializeField]
        [Range(0, 20)]
        private float pupilSpeed = 2;
        
        private GameObject pupil;

        private IEnumerator<float> lookAtEnumerator;
        private Vector2 lastTarget;

        private Circle eye;

        void Awake()
        {
            var t = GetComponent<RectTransform>();
            eye = new Circle(t.sizeDelta.Max(), t.anchoredPosition);
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

            Vector2 target = data.selectedObject.transform.position;
            if (lastTarget == target)
                return;

            lastTarget = target;

            if (lookAtEnumerator != null)
                Timing.KillCoroutines(lookAtEnumerator);

            lookAtEnumerator = Timing.RunCoroutine(_LookAtTarget(target));
        }

        private IEnumerator<float> _LookAtTarget(Vector2 target)
        {
            target = eye.ProjectPoint(target);
            Vector2 currentPos;
            Rect eyeBounds = GetComponent<RectTransform>().rect;
            var transform = pupil.GetComponent<RectTransform>();
            float delay = 1 - pupilSpeed / 20;
            do
            {
                currentPos = Vector2.Lerp(transform.position, target, 0.3f);
                transform.position = currentPos;
                yield return Timing.WaitForSeconds(delay);
            } while (currentPos != target);
        }
        
    }
}