namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.UI;

    using System;
    using System.Collections.Generic;
    using UnityEngine.EventSystems;
    using UnityQuery;
    using UnityEngine.Events;
    using System.Collections;

    [RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    public class MenuEyeController : MonoBehaviour
    {

        [SerializeField]
        private Circle eyeBounds;

        [SerializeField]
        [Range(0.01f, 1)]
        private float movementInterpolationAmount = 0.2f;

        [SerializeField][Range(0,1)]
        private float distanceThreshold = 0.1f;

        public UnityEvent OnBeganMoving;
        public UnityEvent OnTargetReached;

        private Coroutine lookAtCoroutine;

        private GameObject pupil;
        private GameObject Pupil
        {
            get
            {
                if (!pupil)
                    GetPupil();
                return pupil;
            }
        }

        private void GetPupil()
        {
            try
            {
                pupil = transform.FindDescendant("Pupil").gameObject;
            }
            catch (NullReferenceException ex)
            {
                Log.E(this, "Failed to find eye pupil.\n" + ex.Message);
            }
        }

        void Awake()
        {
            var t = GetComponent<RectTransform>(); 
            eyeBounds.center = t.anchoredPosition;
            if (eyeBounds.radius == 0)
                eyeBounds.radius = t.sizeDelta.Min() / 2;
        }

        public void Reset()
        {
            LookAt(Vector2.zero);
        }

        public void LookAt(BaseEventData data)
        {
            if (!Pupil)
                return;
            LookAt(data.selectedObject);
        }

        public void LookAt(GameObject target)
        {
            LookAt(eyeBounds.ProjectPoint(target.GetComponent<RectTransform>().anchoredPosition));
        }

        public void LookAt(Vector2 target)
        {
            if (lookAtCoroutine != null)
                StopCoroutine(lookAtCoroutine);

            lookAtCoroutine = StartCoroutine(_LookAtTarget(target));
        }

        private IEnumerator _LookAtTarget(Vector2 target)
        {
            if (Pupil)
            {
                OnBeganMoving.Invoke();

                var transform = Pupil.GetComponent<RectTransform>();
                Vector2 currentPos = transform.anchoredPosition;
                float initialDistance = Vector2.Distance(currentPos, target);

                do
                {
                    currentPos = Vector3.Slerp(currentPos, target, this.movementInterpolationAmount);
                    transform.anchoredPosition = currentPos;

                    yield return null;
                } while (Vector2.Distance(currentPos, target) / initialDistance >= distanceThreshold);

                transform.anchoredPosition = target;
                OnTargetReached.Invoke();
            }
            else
            {
                Log.E(this, "No game object assigned as pupil");
            }
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}