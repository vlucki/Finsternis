namespace Finsternis
{
    public class ImageFillTransition : FillTransition
    {
        protected override void Awake()
        {
            transitionType = FillType.Fill;
            base.Awake();
        }
#if UNITY_EDITOR
        void OnValidate()
        {
            transitionType = FillType.Fill;
        }
#endif
    }
}