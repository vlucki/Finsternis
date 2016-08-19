using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Finsternis
{
    public class EnemySpawner : MonoBehaviour
    {
        public DungeonDrawer drawer;
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
        }

        public void BeginSpawn(Dungeon dungeon)
        {
            if (!dungeon)
                throw new ArgumentException("Must have a dungeon in order to spawn enemies!");

            if (enemiesHolder)
                Destroy(enemiesHolder);

            enemyDensity = Dungeon.Random.RangePower(0, 0.2f, 0.4f);
            enemiesHolder = new GameObject("Enemies");
            enemyDensity = 0.1f;

            List<KillEnemyGoal> goals = new List<KillEnemyGoal>();
            if (enemies != null && enemies.Count > 0)
            {
                int roomsToSpawn = Dungeon.Random.Range(1, dungeon.Rooms.Count, false);
                do
                {
                    SpawnEnemies(dungeon, goals);
                }
                while (--roomsToSpawn > 0);
            }
        }

        private void SpawnEnemies(Dungeon dungeon, List<KillEnemyGoal> goals)
        {
            Room room = dungeon.GetRandomRoom();
            int enemiesToSpawn = Mathf.CeilToInt(Dungeon.Random.Range(0, room.CellCount * enemyDensity));
            int remainingEnemies = enemiesToSpawn;
            do
            {
                int enemyToSpawn = enemies.Count == 1 ? 0 : Mathf.CeilToInt(Dungeon.Random.Range(0, enemies.Count - 1));
                int remainingEnemiesOfChosenType = Mathf.CeilToInt(Dungeon.Random.Range(0, remainingEnemies));
                if (remainingEnemiesOfChosenType > 0)
                {
                    KillEnemyGoal goal = MakeGoal(dungeon, goals, enemies[enemyToSpawn]);
                    goal.quantity += remainingEnemiesOfChosenType;
                    SpawnEnemy(enemiesHolder.transform, room, goal, remainingEnemiesOfChosenType);
                }
            }
            while (--remainingEnemies > 0);
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
            MTRandom enemyRandom = new MTRandom(goal.enemy.name.GetHashCode());
            EntityAttribute[] enemyAttributes = null;
            do
            {
                Vector2 cell = room.GetRandomCell() + Vector2.one / 2; //center enemy on cell
                GameObject enemy = ((GameObject)Instantiate(goal.enemy, drawer.GetWorldPosition(cell), Quaternion.Euler(0, Random.Range(0, 360), 0)));
                enemy.transform.SetParent(parent);
                enemy.GetComponent<Character>().onDeath.AddListener(goal.EnemyKilled);

                var enemyChar = enemy.GetComponent<Character>();
                if (enemyAttributes == null)
                {
                    enemyAttributes = new EntityAttribute[baseAttributes.Count];
                    for (int i = 0; i < enemyAttributes.Length; i++)
                        enemyAttributes[i] = AddAttribute(enemyChar, baseAttributes[i], enemyRandom);
                    
                }
                else
                {
                    foreach (var attribute in enemyAttributes)
                        enemyChar.AddAttribute(Instantiate(attribute));
                }


            } while (--amount > 0);
        }

        private EntityAttribute AddAttribute(Entity enemy, EntityAttribute attribute, MTRandom rng)
        {
            EntityAttribute enemyAttribute = Instantiate(attribute);

            int value = Mathf.RoundToInt(rng.valuePower(0.8f) * 10);

            if (attribute.LimitMaximum)
            {
                enemyAttribute.SetMax(value);
            }

            enemyAttribute.SetValue(value);

            enemy.AddAttribute(enemyAttribute);

            return enemyAttribute;
        }
    }
}