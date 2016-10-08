namespace Finsternis
{
    using UnityEngine;

    public class EnemyChar : Character
    {        
        protected override void Start()
        {
            Dungeon.Random.SetSeed(name.GetHashCode());
            base.Start();
        }

        protected override void Die()
        {
            if(LastInteraction && LastInteraction.Agent && LastInteraction.Agent.CompareTag("Player"))
            {
                Dungeon.Random.SetSeed(name.GetHashCode());
                FindObjectOfType<CardsManager>().GivePlayerCard(Dungeon.Random.IntRange(1, 4));
            }
            base.Die();
        }

        protected override void InitializeAttribute(int attributeIndex)
        {
            base.InitializeAttribute(attributeIndex);
            var attribute = attributes[attributeIndex];

            int value = Mathf.CeilToInt(Dungeon.Random.value() * 10);

            if (attribute.LimitMaximum)
            {
                attribute.SetMax(value);
            }

            attribute.SetBaseValue(value);
        }
    }
}