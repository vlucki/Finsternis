namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;

    public class EnemyChar : Character
    {
        [SerializeField]
        private RangeI cardsToGive = new RangeI(1, 4);

        protected override void Die()
        {
            if (cardsToGive.max > 0)
            {
                if (LastInteraction && LastInteraction.Agent && LastInteraction.Agent.CompareTag("Player"))
                {
                    GameManager.Instance.CardsManager.GivePlayerCard(Random.Range(cardsToGive.min, cardsToGive.max));
                }
            }
            base.Die();
        }
    }
}