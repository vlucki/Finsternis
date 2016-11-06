namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;

    public class EnemyChar : Character
    {
        protected override void Die()
        {
            if (LastInteraction && LastInteraction.Agent && LastInteraction.Agent.CompareTag("Player"))
            {
                Random.InitState(this.name.GetHashCode());
                FindObjectOfType<CardsManager>().GivePlayerCard(Random.Range(1, 4));
            }
            base.Die();
        }
    }
}