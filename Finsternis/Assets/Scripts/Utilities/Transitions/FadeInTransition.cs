namespace Finsternis
{
    using System;
    using UnityEngine;

    [AddComponentMenu("Finsternis/Transitions/Fade In")]
    public class FadeInTransition : FadeTransition
    {
        protected override void Awake()
        {
            transitionType = "FadeIn";
            base.Awake();
        }
    }
}