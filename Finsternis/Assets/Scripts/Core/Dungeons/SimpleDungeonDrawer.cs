﻿using UnityEngine;
using System.Collections.Generic;
using CellType = SimpleDungeon.CellType;
using UnityEngine.Events;

[RequireComponent(typeof(SimpleDungeon))]
public class SimpleDungeonDrawer : MonoBehaviour
{

    [SerializeField]
    private SimpleDungeon _dungeon;
    public Vector3 scale = Vector3.one;
    public Material defaultWallMaterial;
    public PhysicMaterial defaultWallPhysicMaterial;
    public Material defaultFloorMaterial;
    public PhysicMaterial defaultFloorPhysicMaterial;
    public Vector3 overallOffset = Vector3.zero;

    public GameObject[] walls;
    public GameObject[] doorways;
    public GameObject[] floors;

    private GameObject _floor;

    public UnityEvent onDrawBegin;
    public UnityEvent onDrawEnd;


    public void Start()
    {
        _dungeon = GetComponent<SimpleDungeon>();        
    }

    public void Draw()
    {
        onDrawBegin.Invoke();

        if (_dungeon.customSeed)
            Random.seed = _dungeon.Seed;

        Clear();

        CellType[,] grid = _dungeon.GetDungeon();
        int width = _dungeon.Width;
        int height = _dungeon.Height;


        for (int cellY = -1; cellY <= height; cellY++)
        {
            for (int cellX = -1; cellX <= width; cellX++)
            {
                if (cellY < 0 || cellY == height || cellX < 0 || cellX == width || ShouldMakeWall<CellType>(grid, cellX, cellY, CellType.wall, false))
                {
                    MakeWall(cellX, cellY, grid);
                }
                else
                {
                    MakeFloor(cellX, cellY, grid);
                }
            }
        }


        onDrawEnd.Invoke();
    }

    private void MakeFloor(int cellX, int cellY, CellType[,] grid)
    {

        Vector3 pos = new Vector3(cellX * scale.x + scale.x / 2, 0, -cellY * scale.z - scale.z / 2);
        GameObject floor = MakePlane(pos, new Vector3(scale.x, scale.z, 1), Vector3.right * 90, defaultFloorMaterial, grid[cellY, cellX] + " (" + cellX + ";" + cellY + ")");
        floor.transform.SetParent(gameObject.transform);

        if (cellX == (int)_dungeon.Exit.x && cellY == (int)_dungeon.Exit.y)
        {
            floor.name = "Exit " + floor.name.Remove(0, floor.name.IndexOf('('));
            floor.tag = "Exit";
            floor.AddComponent<Exit>();
            return;
        }
        else
        {
            floor.layer = LayerMask.NameToLayer("Floor");
            if (cellX == (int)_dungeon.Entrance.x && cellY == (int)_dungeon.Entrance.y)
            {
                GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pedestal.name = "Entrance";
                pedestal.transform.SetParent(floor.transform);
                pedestal.transform.localScale = new Vector3(1, 0.05f, 1);
                pedestal.transform.localPosition = Vector3.zero;
#if UNITY_EDITOR
                DestroyImmediate(pedestal.GetComponent<CapsuleCollider>());
#else
                Destroy(pedestal.GetComponent<CapsuleCollider>());
#endif
            }
        }


        Vector2 direction;
        if (doorways != null && doorways.Length > 0 && CanMakeDoorway(cellX, cellY, grid, out direction))
        {

            if (direction.x == 90 || direction.x == 270)
                pos.x += scale.x / 2.5f * direction.y;
            else
                pos.z -= scale.z / 2.5f * direction.y;

            GameObject doorway = Instantiate(doorways[Random.Range(0, doorways.Length)], pos, Quaternion.Euler(Vector3.up * direction.x)) as GameObject;
            doorway.name += "(" + cellX + ";" + cellY + ")";
            doorway.transform.SetParent(gameObject.transform);
        }
    }

    private bool CanMakeDoorway(int cellX, int cellY, CellType[,] grid, out Vector2 direction)
    {
        direction = Vector2.zero;
        Vector2 offset = Vector2.zero;
        int wallsAround = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                int x = cellX + j;
                int y = cellY + i;
                bool isMapEdge = x < 0 || y < 0 || x == grid.GetLength(1) || y == grid.GetLength(0);
                if (isMapEdge || grid[y, x].Equals(CellType.wall))
                {
                    wallsAround++;

                    if(!isMapEdge && Mathf.Abs(i) == Mathf.Abs(j))
                    {
                        offset.x += j;
                        offset.y += i;
                    }
                }
            }
        }

        offset.x = Mathf.Clamp(offset.x, -1, 1);
        offset.y = Mathf.Clamp(offset.y, -1, 1);

        if (wallsAround >= 2 && wallsAround < 6)
        {
            bool wallToTheLeft = (cellX - 1 < 0 || CellType.wall.Equals(grid[cellY, cellX - 1]));
            bool wallToTheRight = (cellX + 1 == grid.GetLength(1) || CellType.wall.Equals(grid[cellY, cellX + 1]));

            bool wallAbove = (cellY - 1 < 0 || CellType.wall.Equals(grid[cellY - 1, cellX]));
            bool wallBelow = (cellY + 1 == grid.GetLength(0) || CellType.wall.Equals(grid[cellY + 1, cellX]));

            if ((wallAbove && wallBelow) | (wallToTheLeft && wallToTheRight))
            {
                if (wallAbove && wallBelow)
                {
                    direction.y = -offset.x;
                    direction.x = (RotateDoorway(cellX, cellY, 0, 1, grid) >= 0) ? 90 : 270;
                }
                else if (wallToTheLeft && wallToTheRight)
                {
                    direction.y = -offset.y;
                    direction.x = (RotateDoorway(cellX, cellY, 1, 0, grid) >= 0) ? 180 : 0;
                }

                return true;
            }
        }
        return false;
    }

    private int RotateDoorway(int x, int y, int xModifier, int yModifier, CellType[,] grid)
    {
        int roomCellsCount = 0;

        for (int i = -1; i < 2; i++)
        {
            int newX = x + i * xModifier;// - yModifier;
            int newY = y + i * yModifier;// - xModifier;
            if ((yModifier == 1 && newY >= 0 && newY < _dungeon.Height)
                || (xModifier == 1 && newX >= 0 && newX < _dungeon.Width))
            {
                if (newX - yModifier >= 0 && newY - xModifier >= 0 && grid[newY - xModifier, newX - yModifier] >= CellType.room) //check left/above
                    roomCellsCount--;

                if (newX + yModifier < _dungeon.Width && newY + xModifier < _dungeon.Height && grid[newY + xModifier, newX + yModifier] >= CellType.room) //check right/below
                    roomCellsCount++;
            }
        }

        return roomCellsCount;
    }

    private void MakeWall(int cellX, int cellY, CellType[,] grid)
    {
        CellType wallType = CellType.wall;
        GameObject wall;
        Vector3 wallPosition = new Vector3(cellX*scale.x, 0, -cellY*scale.y); ;
        if (walls != null && walls.Length > 0)
        {
            wall = walls[Random.Range(0, walls.Length)];
        }
        else
        {
            wall = new GameObject("Wall (" + cellX + ";" + cellY + ")");
            GameObject top = MakePlane(Vector3.up, Vector3.one, Vector3.right * 90, defaultWallMaterial, "WallTop");
            top.transform.SetParent(wall.transform);

            for(int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (Mathf.Abs(i) == Mathf.Abs(j))
                        continue;
                    int x = cellX + j;
                    int y = cellY + i;
                    if (x < 0 || y < 0 || y >= grid.GetLength(0) || x >= grid.GetLength(1))
                        continue;

                    if(!wallType.Equals(grid[y, x]))
                    {
                        float angle = 0;
                        if (x < cellX)
                            angle = 90;
                        else if (x > cellX)
                            angle = -90;
                        else if (y < cellY)
                            angle = 180; //make wall face away the camera
                        GameObject side = MakePlane(new Vector3((float)j/2, 0.5f, -(float)i/2), Vector3.one, new Vector3(0, angle, 0), defaultWallMaterial, "WallSide");
                        side.GetComponent<Collider>().sharedMaterial = defaultWallPhysicMaterial;
                        side.GetComponent<MeshRenderer>().sharedMaterial = defaultWallMaterial;
                        side.transform.SetParent(wall.transform);
                    }
                }
            }

            wall.transform.localScale = scale;
            wallPosition = new Vector3(cellX * scale.x + scale.x/2, 0, -cellY * scale.z - scale.z/2);
        }

        wall.transform.position = wallPosition;
        MergeMeshes(wall, true);
        wall.transform.SetParent(gameObject.transform);
    }

    private GameObject MakePlane(Vector3 pos, Vector3 scale, Vector3 rotation, Material mat, string name = "Ceiling")
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        plane.GetComponent<MeshRenderer>().sharedMaterial = mat;
        plane.transform.localScale = scale;
        plane.transform.rotation = Quaternion.Euler(rotation);
        plane.transform.position = pos;
        plane.name = name;
        return plane;
    }

    private bool ShouldMakeWall<T>(T[,] grid, int cellX, int cellY, T wall, bool ignoreIsolatedWalls)
    {
        return EqualityComparer<T>.Default.Equals(grid[cellY, cellX], wall) && (!ignoreIsolatedWalls || !IsWallIsolated(grid, cellX, cellY, wall));
    }

    private bool IsWallIsolated<T>(T[,] grid, int cellX, int cellY, T wall)
    {
        for(int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (Mathf.Abs(i) == Mathf.Abs(j))
                    continue;
                
                int x = cellX + j;
                int y = cellY + i;

                if(x >= 0 && x < grid.GetLength(1) && y >= 0 && y < grid.GetLength(0) 
                    && !EqualityComparer<T>.Default.Equals(grid[y, x], wall))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void MergeMeshes(GameObject parent, bool nameAfterParent = false, int mergeThreshold = 5)
    {
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length < 1)
            return;

        CombineInstance[] combine = new CombineInstance[System.Math.Min(meshFilters.Length, mergeThreshold)];
        bool combined = false;
        int i = 0;
        int j = 0;
        while (i < meshFilters.Length)
        {
            if (j >= combine.Length)
            {
                AddSection(j, combine);
                combine = new CombineInstance[Mathf.Min(meshFilters.Length - i, mergeThreshold)];
                j = 0;
                combined = true;
            }
            else
            {
                combined = false;
            }
            try
            {
                combine[j].mesh = meshFilters[meshFilters.Length - 1 - i].sharedMesh;
                combine[j].transform = meshFilters[meshFilters.Length - 1 - i].transform.localToWorldMatrix;
                j++;
            }
            catch (System.IndexOutOfRangeException ex)
            {
                Debug.Log(ex);
                Debug.Log(j);
                Debug.Log(i);
                Debug.Log(i - j * meshFilters.Length);
                Debug.Log(meshFilters.Length);
                Debug.Log(combine.Length);
                throw new System.Exception();
            }
            i++;
        }

        if (!combined)
        {
            AddSection(j, combine, nameAfterParent ? parent.name : null);
        }

        foreach (MeshFilter m in meshFilters)
            DestroyImmediate(m.gameObject);

    }

    private void AddSection(int j, CombineInstance[] combine, string name = "")
    {
        if (System.String.IsNullOrEmpty(name))
            name = "section" + j;
        GameObject sectionContainer = new GameObject(name);
        sectionContainer.transform.SetParent(transform);
        Mesh m = new Mesh();
        m.CombineMeshes(combine);
        sectionContainer.AddComponent<MeshFilter>().mesh = m;
        MeshRenderer r = sectionContainer.GetComponent<MeshRenderer>();
        if (!r)
            r = sectionContainer.AddComponent<MeshRenderer>();
        r.material = defaultWallMaterial;
        
        sectionContainer.AddComponent<MeshCollider>().sharedMaterial = defaultWallPhysicMaterial;
    }

    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject g = transform.GetChild(i).gameObject;
            DestroyImmediate(g);
        }
    }


    private void MakeFloor(float width, float height)
    {
        width *= scale.x;
        height *= scale.z;
        _floor = GameObject.CreatePrimitive(PrimitiveType.Quad);
        _floor.name = "floor";
        _floor.transform.localScale = new Vector3(width, height, 1);
        _floor.transform.position = new Vector3(width / 2, 0f, - height / 2);
        _floor.GetComponent<MeshRenderer>().sharedMaterial = defaultFloorMaterial;
        _floor.GetComponent<Collider>().sharedMaterial = defaultFloorPhysicMaterial;
        _floor.transform.SetParent(gameObject.transform);
        _floor.transform.up = Vector3.forward;
    }
}