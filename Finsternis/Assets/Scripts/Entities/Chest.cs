namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;

    [RequireComponent(typeof(Animator))]
    public class Chest : OpeneableEntity
    {
        private Animator animator;

        private int cardsToGive;

        protected override void Awake()
        {
            base.Awake();
            this.animator = GetComponent<Animator>();
            this.cardsToGive = Dungeon.Random.IntRange(1, 3);
        }

        public void OpenChest()
        {
            if (LastInteraction && LastInteraction.Agent.Equals(GameManager.Instance.Player))
            {
                FindObjectOfType<CardsManager>().GivePlayerCard(cardsToGive);
            }
            interactable = false;
            this.animator.SetTrigger("Open");
        }
    }
}