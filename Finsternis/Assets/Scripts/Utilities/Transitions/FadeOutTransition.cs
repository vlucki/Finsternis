namespace Finsternis
{
    using UnityEngine;

    [AddComponentMenu("Finsternis/Transitions/Fade Out")]
    public class FadeOutTransition : FadeTransition
    {
        protected override void Awake()
        {
            transitionType = FadeType.FadeOut;
            base.Awake();
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            transitionType = FadeType.FadeOut;
            base.OnValidate();
        }
#endif
    }

}