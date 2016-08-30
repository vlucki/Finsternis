namespace Finsternis
{
    using UnityEngine;

    [AddComponentMenu("Finsternis/Transitions/Fade Out")]
    public class FadeOutTransition : FadeTransition
    {
        protected override void Awake()
        {
            targetAlpha = 0;
            base.Awake();
        }
    }
}