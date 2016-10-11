namespace Finsternis
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEngine;
    using UnityQuery;
    using Random = UnityEngine.Random;

    public class EnemySpawner : MonoBehaviour
    {
        public DungeonDrawer drawer;
        public List<EntityAttribute> baseAttributes;
        public List<GameObject> enemies;
        public GameObject enemiesHolder;

        [SerializeField]
        [ReadOnly]
        [Tooltip("Set during runtime, by the Dungeon's RNG.")]
        private float baseEnemyDensity = 0.1f;

        void Awake()
        {
            if (!drawer)
                drawer = FindObjectOfType<DungeonDrawer>();
        }

        public void BeginSpawn(Dungeon dungeon)
        {
            if (!dungeon)
                throw new ArgumentException("Must have a dungeon in order to spawn enemies!");

            if (enemiesHolder)
                Destroy(enemiesHolder);

            baseEnemyDensity = 0.01f + Dungeon.Random.value() / 10;
            enemiesHolder = new GameObject("Enemies");

            List<KillEnemyGoal> goals = new List<KillEnemyGoal>();
            int enemiesSpawned = 0;
            if (enemies != null && enemies.Count > 0)
            {
                int roomsToSpawn = dungeon.Rooms.Count - 1; //subtract 1 for the starting room
                do
                {
                    enemiesSpawned += SpawnEnemies(dungeon, goals);
                }
                while (--roomsToSpawn > 0 || enemiesSpawned == 0);
            }
        }

        private int SpawnEnemies(Dungeon dungeon, List<KillEnemyGoal> goals)
        {
            Room room;
            do
            { room = dungeon.GetRandomRoom(); }
            while (room.Equals(dungeon[dungeon.Entrance]));

            int enemiesToSpawn = Mathf.CeilToInt(Dungeon.Random.value() * room.CellCount * (room.Theme.SpawnDensityModifier * this.baseEnemyDensity));
            int enemiesSpawned = 0;
            int remainingEnemies = enemiesToSpawn;
            do
            {
                int remainingEnemiesOfChosenType = Dungeon.Random.IntRange(0, remainingEnemies);
                if (remainingEnemiesOfChosenType > 0)
                {
                    KillEnemyGoal goal = MakeGoal(dungeon, goals, enemies.GetRandom(Dungeon.Random.IntRange));
                    goal.quantity += remainingEnemiesOfChosenType;
                    SpawnEnemy(enemiesHolder.transform, room, goal, remainingEnemiesOfChosenType);
                    enemiesSpawned += remainingEnemiesOfChosenType;
                }
                remainingEnemies -= Mathf.Max(1, remainingEnemiesOfChosenType);
            }
            while (remainingEnemies > 0);

            return enemiesSpawned;
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