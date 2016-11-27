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
                    var givenCards = Random.Range(cardsToGive.min, cardsToGive.max);
                    GameManager.Instance.CardsManager.GivePlayerCard(givenCards);
                    GameManager.Instance.Player.GetCachedComponent<Inventory>().AddPoints(Random.Range(givenCards, givenCards * 2));
                }
            }
            base.Die();
        }
    }
}