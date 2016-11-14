namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;

    public class EnemyChar : Character
    {
        [System.Serializable]
        public struct RangeI
        {
            public
#if !UNITY_EDITOR
                readonly 
#endif
                int minCardsToGive;

            public
#if !UNITY_EDITOR
                readonly 
#endif
                int maxCardsToGive;
        
            public RangeI(int min, int max)
            {
                this.minCardsToGive = min;
                this.maxCardsToGive = max;
            }
        }

        [SerializeField]
        private RangeI cardsToGive = new RangeI(1, 4);

        protected override void Die()
        {
            if (LastInteraction && LastInteraction.Agent && LastInteraction.Agent.CompareTag("Player"))
            {
                GameManager.Instance.CardsManager.GivePlayerCard(Random.Range(cardsToGive.minCardsToGive, cardsToGive.maxCardsToGive));
            }
            base.Die();
        }
    }
}