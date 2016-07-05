using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public Dungeon dungeon;
    public DungeonDrawer drawer;
    public List<GameObject> enemies;
    public GameObject enemyHudPrefab;

    [Range(0.01f, 1)]
    public float enemyDensity = 0.1f;

    void Awake()
    {
        if (!drawer)
            drawer = GetComponent<DungeonDrawer>();
    }

    public void BeginSpawn()
    {
        dungeon = FindObjectOfType<Dungeon>();
        if (!dungeon)
        {
            throw new ArgumentException("Failed to locate dungeon on scene!");
        }

        List<KillEnemyGoal> goals = new List<KillEnemyGoal>();
        if (enemies != null && enemies.Count > 0)
        {
            int roomsToSpawn = dungeon.Random.Range(1, dungeon.Rooms.Count, false);
            do
            {
                SpawnEnemies(goals);
            }
            while (--roomsToSpawn > 0);
        }
    }

    private void SpawnEnemies(List<KillEnemyGoal> goals)
    {
        Room room = dungeon.GetRandomRoom();
        int enemiesToSpawn = Mathf.CeilToInt(dungeon.Random.Range(0, room.CellCount * enemyDensity));
        int remainingEnemies = enemiesToSpawn;
        do
        {
            int enemyToSpawn = enemies.Count == 1 ? 0 : Mathf.CeilToInt(dungeon.Random.Range(0, enemies.Count - 1));
            int remainingEnemiesOfChosenType = Mathf.CeilToInt(dungeon.Random.Range(0, remainingEnemies));
            if (remainingEnemiesOfChosenType > 0)
            {
                KillEnemyGoal goal = MakeGoal(goals, enemies[enemyToSpawn]);
                goal.quantity += remainingEnemiesOfChosenType;
                SpawnEnemy(room, goal, remainingEnemiesOfChosenType);
            }
        }
        while (--remainingEnemies > 0);
    }

    private KillEnemyGoal MakeGoal(List<KillEnemyGoal> goals, GameObject enemy)
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

    private void SpawnEnemy(Room room, KillEnemyGoal goal, int amount)
    {
        do
        {
            Vector2 cell = room.GetRandomCell() + Vector2.one;
            GameObject enemy = ((GameObject)Instantiate(goal.enemy, new Vector3(cell.x * drawer.overallScale.x, 0.2f, -cell.y * drawer.overallScale.y), Quaternion.Euler(0, Random.Range(0, 360), 0)));
            enemy.transform.SetParent(transform);
            enemy.GetComponent<EnemyController>().onDeath.AddListener(goal.EnemyKilled);
        } while (--amount > 0);
    }
}