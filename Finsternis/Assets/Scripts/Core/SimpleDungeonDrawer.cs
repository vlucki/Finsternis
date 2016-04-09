using UnityEngine;
using System.Collections.Generic;
using CellType = SimpleDungeon.CellType;

[RequireComponent(typeof(SimpleDungeon))]
public class SimpleDungeonDrawer : MonoBehaviour
{

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

    [SerializeField]
    private SimpleDungeon _dungeon;

    public void Start()
    {
        _dungeon = GetComponent<SimpleDungeon>();        
    }

    public void Draw()
    {
        Clear();

        CellType[,] grid = _dungeon.GetDungeon();
        int width = _dungeon.Width;
        int height = _dungeon.Height;


        for (int cellY = -1; cellY <= height; cellY++)
        {
            for (int cellX = -1; cellX <= width; cellX++)
            {
                if (cellY < 0 || cellY == height || cellX < 0 || cellX == width || ShouldMakeWall<CellType>(grid, cellX, cellY, CellType.empty, false))
                {
                    MakeWall(cellX, cellY, grid);
                }
                else
                {
                    MakeFloor(cellX, cellY, grid);
                }
            }
        }
    }

    private void MakeFloor(int cellX, int cellY, CellType[,] grid)
    {

        Vector3 pos = new Vector3(cellX * scale.x + scale.x / 2, 0, -cellY * scale.z - scale.z / 2);
        GameObject floor = MakePlane(pos, new Vector3(scale.x, scale.z, 1), Vector3.right * 90, defaultFloorMaterial, grid[cellY, cellX] + " (" + cellX + ";" + cellY + ")");
        floor.transform.SetParent(gameObject.transform);
         
        if (cellX == (int)_dungeon.End.x && cellY == (int)_dungeon.End.y)
        {
            floor.GetComponent<MeshRenderer>().enabled = false;
#if UNITY_EDITOR
            DestroyImmediate(floor.GetComponent<MeshCollider>());
#else
            Destroy(floor.GetComponent<MeshCollider());
#endif
            BoxCollider collider = floor.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(1, 1, 1);
            collider.center = Vector3.forward / 2;

            return;
        }


        Vector2 direction;
        if (doorways != null && doorways.Length > 0 && CanMakeDoorway(cellX, cellY, grid, out direction))
        {
            GameObject portal = GameObject.Instantiate<GameObject>(doorways[Random.Range(0, doorways.Length)]);
            if (direction.x == 1)
            {
                portal.transform.rotation = Quaternion.Euler(Vector3.up * 90);
                pos.x += scale.x / 2.5f * direction.y;
            }
            else
            {
                pos.z -= scale.z / 2.5f * direction.y;
            }

            portal.transform.position = pos;
            portal.name += "(" + cellX + ";" + cellY + ")";
            portal.transform.SetParent(gameObject.transform);
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
                if (isMapEdge || grid[y, x].Equals(CellType.empty))
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

        if (wallsAround < 2 || wallsAround >= 6)
            return false;

        bool wallToTheLeft = (cellX - 1 < 0 || CellType.empty.Equals(grid[cellY, cellX - 1]));
        bool wallToTheRight = (cellX + 1 == grid.GetLength(1) || CellType.empty.Equals(grid[cellY, cellX + 1]));

        bool wallAbove = (cellY - 1 < 0 || CellType.empty.Equals(grid[cellY - 1, cellX]));
        bool wallBelow = (cellY + 1 == grid.GetLength(0) || CellType.empty.Equals(grid[cellY + 1, cellX]));

        if ((wallAbove && wallBelow) | (wallToTheLeft && wallToTheRight))
        {
            direction.x = (wallAbove && wallBelow) ? 1 : 0; //set rotation

            if (wallAbove && wallBelow)
            {
                direction.y = -offset.x;
            }
            else
            {
                direction.y = -offset.y;
            }


            return true;
        }

        return false;
    }

    private void MakeWall(int cellX, int cellY, CellType[,] grid)
    {
        CellType wallType = CellType.empty;
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
        if (meshFilters.Length < 2)
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
