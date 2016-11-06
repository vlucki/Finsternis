namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;

    public class EnemyChar : Character
    {
        static int enemyCount = -1;
        int enemyID = 0;

        protected override void Start()
        {
            enemyCount++;
            this.enemyID = enemyCount;

            Random.InitState(this.name.GetHashCode());

            base.Start();
#if LOG_INFO
            Log.Info(this, "Attributes for ID: {0} are {1}", this.enemyID, this.attributes.SequenceToString());
#endif
        }

        protected override void InitializeAttribute(int attributeIndex)
        {
            base.InitializeAttribute(attributeIndex);
            var attribute = attributes[attributeIndex];

            int value = Mathf.CeilToInt(Random.value * 10);

            if (attribute.LimitMaximum)
            {
                attribute.SetMax(value);
            }

            attribute.SetBaseValue(value);
        }

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