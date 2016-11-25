namespace Finsternis
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityQuery;
    using Random = UnityEngine.Random;

    public class EnemySpawner : MonoBehaviour
    {
        public DungeonDrawer drawer;
        public List<EntityAttribute> baseAttributes;
        public List<GameObject> enemies;
        public GameObject enemiesHolder;

        public UnityEvent onFinishedSpawning;

        [SerializeField, ReadOnly, Tooltip("Set during runtime, by the Dungeon's RNG.")]
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

            StopAllCoroutines();
            StartCoroutine(_Spawn(dungeon));
        }

        private IEnumerator _Spawn(Dungeon dungeon)
        {
            if (!GameManager.Instance.Player)
                yield return new WaitWhile(() => !GameManager.Instance.Player);

            if(!dungeon)
                yield break;

            baseEnemyDensity = 0.01f + Random.value / 10;
            enemiesHolder = new GameObject("Enemies");

            List<KillEnemyGoal> goals = new List<KillEnemyGoal>();
            int enemiesSpawned = 0;
            if (enemies != null && enemies.Count > 0)
            {
                int roomsToSpawn = dungeon.Rooms.Count - 1; //subtract 1 for the starting room
                Loop.Do(
                    () => (--roomsToSpawn) > 0,
                    () => enemiesSpawned += SpawnEnemies(dungeon, goals)
                    );

#if LOG_INFO
                Log.I(this,
                    "Managed to spawn {0} enemies, rooms ignored = {1}",
                    enemiesSpawned,
                    roomsToSpawn);
#endif
                yield return null;
            }
            onFinishedSpawning.Invoke();
        }

        private int SpawnEnemies(Dungeon dungeon, List<KillEnemyGoal> goals)
        {
            Room room = dungeon.GetRandomRoom(1);

            int enemiesToSpawn = Mathf.CeilToInt(Random.value * room.CellCount * (room.Theme.SpawnDensityModifier * this.baseEnemyDensity));
            int enemiesSpawned = 0;
            int remainingEnemies = enemiesToSpawn;

            int maxEnemyIndex = Mathf.Min(this.enemies.Count - 1, GameManager.Instance.ClearedDungeons + 1); //show more enemies at later dungeons

            Loop.Do(
                () => remainingEnemies > 0,
                () =>
                {
                    int remainingEnemiesOfChosenType = remainingEnemies == 1 ? 1 : Random.Range(1, remainingEnemies);

                    KillEnemyGoal goal = MakeGoal(dungeon, goals, enemies.GetRandom(Random.Range, 0, maxEnemyIndex));
                    goal.quantity += remainingEnemiesOfChosenType;
                    SpawnEnemyOfType(enemiesHolder.transform, room, goal, remainingEnemiesOfChosenType);
                    enemiesSpawned += remainingEnemiesOfChosenType;
                    remainingEnemies -= remainingEnemiesOfChosenType;

                }
                );

            return enemiesSpawned;
        }

        private KillEnemyGoal MakeGoal(Dungeon dungeon, List<KillEnemyGoal> goals, GameObject enemy)
        {
            KillEnemyGoal goal = goals.Find((g) => g.enemy.Equals(enemy));

            if (!goal)
            {
                goal = dungeon.AddGoal<KillEnemyGoal>();
                goals.Add(goal);

                goal.enemy = enemy;
            }

            return goal;
        }

        private void SpawnEnemyOfType(Transform parent, Room room, KillEnemyGoal goal, int amount)
        {
            HashSet<Vector2> usedCells = new HashSet<Vector2>();
            float dungeonProgress = (1f + GameManager.Instance.ClearedDungeons) / GameManager.Instance.DungeonsToClear;
            
            do
            {
                Vector2 cell = room.GetRandomCell();

                //Try to avoid spawning enemies on top of eachother (since physics tend to throw them all over the place)
                Loop.Do(() => usedCells.Contains(cell),
                    () => cell = room.GetRandomCell(), room.CellCount);
                usedCells.Add(cell);
                cell += Vector2.one / 2; //center enemy on cell
                GameObject enemy = ((GameObject)Instantiate(goal.enemy, drawer.GetWorldPosition(cell).WithY(.1f), Quaternion.Euler(0, Random.Range(0, 360), 0)));
                enemy.transform.SetParent(parent);
                var enemyChar = enemy.GetComponent<EnemyChar>();
                enemyChar.onDeath.AddListener(goal.EnemyKilled);

                //make enemies stronger the more dungeons are cleared
                enemyChar.onAttributeInitialized.AddListener(
                    attribute =>
                    {
                        if (attribute.HasMaximumValue)
                            attribute.SetMax(attribute.Max * dungeonProgress);
                        attribute.SetBaseValue(attribute.BaseValue * dungeonProgress);

                    });

            } while (--amount > 0);
        }
    }
}