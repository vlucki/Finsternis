using UnityEngine;
using System.Collections;

namespace Finsternis
{
    public class EnemyChar : Character
    {
        private IRandom enemyRandom;

        protected override void Awake()
        {
            enemyRandom = Dungeon.Random;
            enemyRandom.SetSeed(name.GetHashCode());
            base.Awake();
        }

        protected override void InitializeAttribute(int attributeIndex)
        {
            base.InitializeAttribute(attributeIndex);
            var attribute = attributes[attributeIndex];

            int value = Mathf.CeilToInt(enemyRandom.value() * 10);

            if (attribute.LimitMaximum)
            {
                attribute.SetMax(value);
            }

            attribute.SetValue(value);
        }
    }
}