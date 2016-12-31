namespace Finsternis
{
    using UnityEngine;

    public class EnemyChar : Character
    {
        [SerializeField]
        private RangeI cardsToGive = new RangeI(1, 4);

        protected override void Die()
        {
            if (cardsToGive.max > 0)
            {
                var givenCards = Random.Range(cardsToGive.min, cardsToGive.max);
                if (givenCards > 0)
                {
                    GameManager.Instance.CardsManager.GivePlayerCard(givenCards);
                    GameManager.Instance.Player.GetComponent<Inventory>().AddPoints(Random.Range(givenCards / 2, givenCards));
                }

            }
            base.Die();
        }
    }
}