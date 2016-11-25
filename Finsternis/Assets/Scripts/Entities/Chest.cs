namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;

    [RequireComponent(typeof(Animator))]
    public class Chest : OpeneableEntity
    {
        private Animator animator;

        [Serializable]
        public struct RangedValue
        {
            public int min;
            public int max;

            public RangedValue(int min, int max)
            {
                this.min = min;
                this.max = max;
            }
        }

        [SerializeField]
        private RangedValue rangeOfCardsToGive = new RangedValue(1, 3);
        
        [SerializeField]
        private Material glowMaterial;

        private int cardsToGive;

        protected override void Awake()
        {
            base.Awake();
            this.animator = GetComponent<Animator>();
            this.cardsToGive = UnityEngine.Random.Range(this.rangeOfCardsToGive.min, this.rangeOfCardsToGive.max);
            var renderers = this.GetComponentsInChildren<Renderer>();
            float percentage = (float)this.cardsToGive / this.rangeOfCardsToGive.max;
            Color color = new Color(percentage, percentage, percentage);
            foreach (var renderer in renderers)
            {
                foreach(var mat in renderer.materials)
                {
                    if (mat.name.Substring(0, mat.name.IndexOf("(")).TrimEnd().Equals(this.glowMaterial.name))
                        mat.SetColor("_EmissionColor", color);
                }
            }
        }

        public override void Interact(EntityAction action)
        {
            if(action is OpenAction)
                base.Interact(action);
        }

        public override void Open()
        {
            base.Open();
            if (!IsOpen)
                return;

            if (LastInteraction && LastInteraction.Agent.Equals(GameManager.Instance.Player.Character))
            {
                GameManager.Instance.CardsManager.GivePlayerCard(cardsToGive);
            }
            interactable = false;
            this.animator.SetTrigger("opening");
        }

        void OnValidate()
        {
            this.rangeOfCardsToGive.min = Mathf.Min(this.rangeOfCardsToGive.min, this.rangeOfCardsToGive.max);
        }
    }
}