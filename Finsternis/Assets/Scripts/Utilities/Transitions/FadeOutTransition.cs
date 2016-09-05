namespace Finsternis
{
    using UnityEngine;

    [AddComponentMenu("Finsternis/Transitions/Fade Out")]
    public class FadeOutTransition : FadeTransition
    {
        protected override void Awake()
        {
            transitionType = "FadeOut";
            base.Awake();
        }

    }
}