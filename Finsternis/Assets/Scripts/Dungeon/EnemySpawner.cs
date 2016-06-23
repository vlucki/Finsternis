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
        if (enemies != null && enemies.Count > 0)
        {
            int roomsToSpawn = dungeon.Random.Range(1, dungeon.Rooms.Count, false);
            do
            {
                Room r = dungeon.GetRandomRoom();
                int enemiesToSpawn = Mathf.CeilToInt(dungeon.Random.Range(0, r.CellCount * enemyDensity));
                do
                {
                    int enemyToSpawn = enemies.Count == 1 ? 0 : Mathf.CeilToInt(dungeon.Random.Range(0, enemies.Count-1));
                    dungeon.killsUntilNext++;
                    Vector2 cell = r.GetRandomCell() + Vector2.one;
                    GameObject enemy = ((GameObject)Instantiate(enemies[enemyToSpawn], new Vector3(cell.x * drawer.overallScale.x, 0.2f, -cell.y * drawer.overallScale.y), Quaternion.Euler(0, Random.Range(0, 360), 0)));
                    enemy.transform.SetParent(transform);
                    enemy.GetComponent<Character>().onDeath.AddListener(dungeon.EnemyKilled);
                }
                while (--enemiesToSpawn > 0);
            }
            while (--roomsToSpawn > 0);
        }
    }
}