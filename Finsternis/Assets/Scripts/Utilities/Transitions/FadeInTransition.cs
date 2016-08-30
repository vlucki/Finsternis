namespace Finsternis
{
    using UnityEngine;

    [AddComponentMenu("Finsternis/Transitions/Fade In")]
    public class FadeInTransition : FadeTransition
    {
        protected override void Awake()
        {
            targetAlpha = 1;
            base.Awake();
        }
    }
}