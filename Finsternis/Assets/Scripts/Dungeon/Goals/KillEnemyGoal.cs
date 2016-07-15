using UnityEngine;

namespace Finsternis
{
    public class KillEnemyGoal : DungeonGoal
    {
        public GameObject enemy;
        public int quantity;

        public override void Check()
        {
            if (quantity <= 0)
            {
                goalReached = true;
                onGoalReached.Invoke(this);
            }
        }

        public void EnemyKilled()
        {
            if (!goalReached)
            {
                quantity--;
                Check();
            }
        }
    }
}