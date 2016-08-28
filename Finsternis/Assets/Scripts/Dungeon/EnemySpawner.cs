namespace Finsternis
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEngine;

    using Random = UnityEngine.Random;

    public class EnemySpawner : MonoBehaviour
    {
        public DungeonDrawer drawer;
        private CardsManager cardsManager;
        public List<EntityAttribute> baseAttributes;
        public List<GameObject> enemies;
        public GameObject enemiesHolder;

        [SerializeField]
        [ReadOnly]
        [Tooltip("Set during runtime, by the Dungeon's RNG.")]
        private float enemyDensity = 0.1f;

        void Awake()
        {
            if (!drawer)
                drawer = FindObjectOfType<DungeonDrawer>();

            cardsManager = FindObjectOfType<CardsManager>();
        }

        public void BeginSpawn(Dungeon dungeon)
        {
            if (!dungeon)
                throw new ArgumentException("Must have a dungeon in order to spawn enemies!");

            if (enemiesHolder)
                Destroy(enemiesHolder);

            enemyDensity = Dungeon.Random.Range(0, 0.2f);
            enemiesHolder = new GameObject("Enemies");

            List<KillEnemyGoal> goals = new List<KillEnemyGoal>();
            if (enemies != null && enemies.Count > 0)
            {
                int roomsToSpawn = Dungeon.Random.IntRange(1, dungeon.Rooms.Count, false);
                do
                {
                    SpawnEnemies(dungeon, goals);
                }
                while (--roomsToSpawn > 0);
            }
        }

        private void SpawnEnemies(Dungeon dungeon, List<KillEnemyGoal> goals)
        {
            Room room;
            do
            { room = dungeon.GetRandomRoom(); }
            while (room.Equals(dungeon[dungeon.Entrance]));

            int enemiesToSpawn = Mathf.CeilToInt(Dungeon.Random.Range(0, room.CellCount * enemyDensity));

            enemiesToSpawn = Mathf.Min(enemiesToSpawn, room.CellCount);

            int remainingEnemies = enemiesToSpawn;
            do
            {
                int enemyToSpawn = enemies.Count == 1 ? 0 : Dungeon.Random.IntRange(0, enemies.Count, false);
                int remainingEnemiesOfChosenType = Dungeon.Random.IntRange(0, remainingEnemies, true);
                if (remainingEnemiesOfChosenType > 0)
                {
                    KillEnemyGoal goal = MakeGoal(dungeon, goals, enemies[enemyToSpawn]);
                    goal.quantity += remainingEnemiesOfChosenType;
                    SpawnEnemy(enemiesHolder.transform, room, goal, remainingEnemiesOfChosenType);
                }
                --remainingEnemies;
            }
            while (remainingEnemies > 0);
        }

        private KillEnemyGoal MakeGoal(Dungeon dungeon, List<KillEnemyGoal> goals, GameObject enemy)
        {
            KillEnemyGoal goal = null;
            foreach (KillEnemyGoal g in goals)
            {
                if (g.enemy.Equals(enemy))
                {
                    goal = g;
                    break;
                }
            }

            if (!goal)
            {
                goal = dungeon.AddGoal<KillEnemyGoal>();
                goals.Add(goal);

                goal.enemy = enemy;
            }

            return goal;
        }

        private void SpawnEnemy(Transform parent, Room room, KillEnemyGoal goal, int amount)
        {
            do
            {
                Vector2 cell = room.GetRandomCell() + Vector2.one / 2; //center enemy on cell
                GameObject enemy = ((GameObject)Instantiate(goal.enemy, drawer.GetWorldPosition(cell), Quaternion.Euler(0, Random.Range(0, 360), 0)));
                enemy.transform.SetParent(parent);
                enemy.GetComponent<EnemyChar>().onDeath.AddListener(goal.EnemyKilled);
            } while (--amount > 0);
        }
    }
}