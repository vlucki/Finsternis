namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityQuery;
    using UnityEngine.UI;
    using System.Collections.Generic;

    [RequireComponent(typeof(CanvasGroup), typeof(InputRouter))]
    [DisallowMultipleComponent]
    public class InGameMenuController : MonoBehaviour
    {

        private static int InstanceCount = 0;

        private CanvasGroup group;
        private List<Button> options;
        private MenuEyeController eyeController;

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
                options = new List<Button>();
                GetComponentsInChildren<Button>(options);
                if(options.Count <= 0)
                    Log.Warn("Not a single option found on the menu.");
                
            }
        }
    }
}