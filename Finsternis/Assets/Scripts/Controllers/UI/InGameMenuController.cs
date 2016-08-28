namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityQuery;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using UnityEngine.EventSystems;
    using MovementEffects;
    using System;

    [RequireComponent(typeof(CanvasGroup), typeof(InputRouter))]
    [DisallowMultipleComponent]
    public class InGameMenuController : MonoBehaviour
    {
        private static int InstanceCount = 0;

        private CanvasGroup group;
        private List<Button> options;
        private Circle menuBounds;
        private bool togglingMenu;
        private EventSystem evtSys;

        void Awake()
        {
            if (InstanceCount > 0)
            {
                Log.Warn("There should only be one instance of the InGame menu at any given time.");
                Destroy(this.gameObject);
            }
            else
            {
                this.evtSys = FindObjectOfType<EventSystem>();
                this.group = GetComponent<CanvasGroup>();
                this.menuBounds = new Circle(GetComponent<RectTransform>().sizeDelta.Min()/2, GetComponent<RectTransform>().anchoredPosition);
                this.options = new List<Button>();
                GetComponentsInChildren<Button>(options);
                if (options.Count <= 0)
                    Log.Warn("Not a single option found on the menu.");
                else
                    PositionButtons();
                
            }
        }

        public void Open()
        {

        }

        public void Close()
        {

        }

        private void _ToggleMenu(bool opening)
        {
            float targetPercentage = opening ? 1 : 0;
            float currentPercentage = 1 - targetPercentage;
            do
            {
                PositionButtons(currentPercentage);
                currentPercentage = Mathf.Lerp(currentPercentage, targetPercentage, 0.2f);
            }
            while (!Mathf.Approximately(currentPercentage, targetPercentage));
        }

        private void PositionButtons(float percentage = 1)
        {
            float angleBetweenOptions = -360 / options.Count;
            Vector2 optionPosition = menuBounds.center + Vector2.up * menuBounds.radius;
            for(int index = 0; index < options.Count; index++)
            {
                options[index].GetComponent<RectTransform>().anchoredPosition = optionPosition;
                var dir = optionPosition - menuBounds.center;
                dir = dir.Rotate(angleBetweenOptions);
                optionPosition = dir.normalized * menuBounds.radius * percentage;
            }
        }
    }
}