namespace Finsternis
{
    public class ReverseImageFillTransition : FillTransition
    {
        protected override void Awake()
        {
            transitionType = FillType.ReverseFill;
            base.Awake();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            transitionType = FillType.ReverseFill;
        }
#endif
    }
}