using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimpleDungeon))]
public class EnemySpawner : MonoBehaviour
{
    public SimpleDungeon dungeon;
    public SimpleDungeonDrawer drawer;
    public List<GameObject> enemies;
    public GameObject enemyHudPrefab;

    [Range(0.01f, 1)]
    public float enemyDensity = 0.1f;

    void Awake()
    {
        if (!dungeon)
            dungeon = GetComponent<SimpleDungeon>();

        if (!drawer)
            drawer = GetComponent<SimpleDungeonDrawer>();

        drawer.onDrawEnd.AddListener(DoSpawn);
    }

    void DoSpawn()
    {
        int roomsToSpawn = dungeon.Random.Range(1, dungeon.Rooms.Count, false);
        int killsRequired = 0;
        do
        {
            Room r = dungeon.GetRandomRoom();
            int enemiesToSpawn = Mathf.CeilToInt(dungeon.Random.Range(0, r.CellCount * enemyDensity));
            do
            {
                if(enemies != null && enemies.Count > 0)
                {
                    Vector2 cell = r.GetRandomCell() + Vector2.one;
                    GameObject enemy = ((GameObject)Instantiate(enemies[0], new Vector3(cell.x * drawer.overallScale.x, 0.2f, -cell.y * drawer.overallScale.y), Quaternion.Euler(0, Random.Range(0, 360), 0)));
                    enemy.transform.SetParent(transform);
                    enemy.GetComponent<Entity>().onDeath.AddListener(dungeon.EnemyKilled);
                    killsRequired++;
                }
            }
            while (--enemiesToSpawn > 0);
        }
        while (--roomsToSpawn > 0);

        dungeon.killsUntilNext = killsRequired;
    }
}