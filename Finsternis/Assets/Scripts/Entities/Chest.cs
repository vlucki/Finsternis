namespace Finsternis
{
    using UnityEngine;
    using Extensions;

    [RequireComponent(typeof(Animator))]
    public class Chest : OpeneableEntity
    {
        private Animator animator;

        [SerializeField]
        private RangeI rangeOfCardsToGive = new RangeI(1, 3);
        
        [SerializeField]
        private Material glowMaterial;

        private int cardsToGive;

        protected override void Awake()
        {
            base.Awake();
            this.animator = GetComponent<Animator>();
            this.cardsToGive = UnityEngine.Random.Range(this.rangeOfCardsToGive.min, this.rangeOfCardsToGive.max);
            float percentage = (float)this.cardsToGive / this.rangeOfCardsToGive.max;
            Color color = new Color(percentage, percentage, percentage);
            SetGlow(color);
        }

        private void SetGlow(Color color)
        {
            var renderers = this.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                foreach (var mat in renderer.materials)
                {
                    if (mat.name.Substring(0, mat.name.IndexOf("(")).TrimEnd().Equals(this.glowMaterial.name))
                        mat.SetColor("_EmissionColor", color);
                }
            }
        }

        public override void Open()
        {
            base.Open();
            if (!IsOpen)
                return;
            
            this.animator.SetTrigger("opening");
        }

        void OnValidate()
        {
            this.rangeOfCardsToGive.min = Mathf.Min(this.rangeOfCardsToGive.min, this.rangeOfCardsToGive.max);
        }
    }
}