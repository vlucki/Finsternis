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

        void Awake()
        {
            if (InstanceCount > 0)
            {
                Log.Warn("There should only be one instance of the InGame menu at any given time.");
                Destroy(this.gameObject);
            }
            else
            {
                group = GetComponent<CanvasGroup>();
                menuBounds = new Circle(GetComponent<RectTransform>().sizeDelta.Min()/2, GetComponent<RectTransform>().anchoredPosition);
                options = new List<Button>();
                GetComponentsInChildren<Button>(options);
                if (options.Count <= 0)
                    Log.Warn("Not a single option found on the menu.");
                else
                    PositionButtons();
                
            }
        }

        private void PositionButtons()
        {
            float angleBetweenOptions = -360 / options.Count;
            Vector2 optionPosition = menuBounds.center + Vector2.up * menuBounds.radius;
            for(int index = 0; index < options.Count; index++)
            {
                options[index].GetComponent<RectTransform>().anchoredPosition = optionPosition;
                var dir = optionPosition - menuBounds.center;
                dir = dir.Rotate(angleBetweenOptions);
                optionPosition = dir.normalized * menuBounds.radius;
            }
        }
    }
}