using UnityEngine;
using CellType = SimpleDungeon.CellType;
using UnityEngine.Events;

[RequireComponent(typeof(SimpleDungeon))]
public class SimpleDungeonDrawer : MonoBehaviour
{
    public Material corridorMaterial;

    [SerializeField]
    private SimpleDungeon _dungeon;
    [Tooltip("Only of walls and floors generated through primitives.")]
    public Vector3 overallScale = Vector3.one;
    public float extraWallHeight = 3;
    public Material defaultWallMaterial;
    public PhysicMaterial defaultWallPhysicMaterial;
    public Material defaultFloorMaterial;
    public PhysicMaterial defaultFloorPhysicMaterial;
    public Vector3 overallOffset = Vector3.zero;

    public GameObject[] walls;
    public GameObject[] doorways;
    public GameObject[] floorTiles;
    public GameObject[] floorTraps;

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
        Clear();
        
        int width = _dungeon.Width;
        int height = _dungeon.Height;


        for (int cellY = -1; cellY <= height; cellY++)
        {
            for (int cellX = -1; cellX <= width; cellX++)
            {
                if (cellY < 0 || cellY == height || cellX < 0 || cellX == width || ShouldMakeWall(cellX, cellY, false))
                {
                    MakeWall(cellX, cellY);
                }
                else
                {
                    MakeFloor(cellX, cellY);
                }
            }
        }

        foreach(Corridor corridor in _dungeon.Corridors)
        {
            if(corridor.Length > 1)
            {
                Vector2 direction = corridor.Direction;
                direction.y -= direction.x;
                direction.x *= 90;

                GameObject prefab = doorways[Random.Range(0, doorways.Length)];

                for (int i = 0; i < 2; i++)
                {
                    Vector3 pos;
                    if(i == 0)  pos = new Vector3(corridor.Bounds.x, 0, corridor.Bounds.y);
                    else        pos = new Vector3(corridor.LastCell.x, 0, corridor.LastCell.y);
                    string name = pos.x.ToString("F0") + ";" + pos.z.ToString("F0");

                    if (CanMakeDoorway((int)pos.x, (int)pos.z))
                    {
                        pos.x *= overallScale.x;
                        pos.x += overallScale.x / 2;

                        pos.z *= -overallScale.z;
                        pos.z -= overallScale.z / 2;


                        if (direction.x == 90 || direction.x == 270)
                        {
                            pos.x += overallScale.x / 2.5f * direction.y;
                            if (direction.x == 270)
                                pos.x += (overallScale.x * 0.8f);
                        }
                        else
                        {
                            pos.z -= overallScale.z / 2.5f * direction.y;
                            if (direction.x == 0)
                                pos.z += (overallScale.z * 0.8f);
                        }

                        GameObject doorway = Instantiate(prefab, pos, Quaternion.Euler(Vector3.up * -direction.x)) as GameObject;
                        doorway.name += "(" + name + ")";
                        doorway.transform.SetParent(gameObject.transform);
                    }
                    direction.x += 180;
                }
            }
        }

        onDrawEnd.Invoke();
    }

    private void MakeFloor(int cellX, int cellY)
    {
        GameObject floor = null;
        string nameSuffix = " (" + cellX + ";" + cellY + ")";
        Vector3 pos = new Vector3(cellX * overallScale.x + overallScale.x / 2, 0, -cellY * overallScale.z - overallScale.z / 2);
        if (_dungeon[cellX, cellY] < (int)CellType.trappedFloor)
        {
            floor = MakePlane(pos, 
                new Vector3(overallScale.x, overallScale.z, 1), 
                Vector3.right * 90, _dungeon[cellX, cellY] == (int)CellType.corridor ? corridorMaterial : defaultFloorMaterial, 
                (CellType)_dungeon[cellX, cellY] + nameSuffix);

            if (cellX == (int)_dungeon.Exit.x && cellY == (int)_dungeon.Exit.y)
            {
                floor.name = "Exit " + nameSuffix;
                floor.tag = "Exit";
                floor.AddComponent<Exit>();
            }
            else
            {
                if (floorTiles != null && floorTiles.Length > 0 && Random.value > 0.5f)
                {
                    floor = Instantiate(floorTiles[Random.Range(0, floorTiles.Length)], pos, Quaternion.Euler(0, 90 * Random.Range(0, 4), 0)) as GameObject;
                    floor.transform.localScale = overallScale;
                }
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
        }
        else
        {
            if (floorTraps != null && floorTraps.Length > 0)
            {
                floor = Instantiate<GameObject>(floorTraps[_dungeon[cellX, cellY] - (int)CellType.trappedFloor]);
                floor.GetComponent<Trap>().Init(new Vector2(cellX, cellY));
                floor.transform.position = pos;
                floor.transform.name += nameSuffix;
                floor.transform.localScale = overallScale;
            }
        }
        if (floor)
        {
            floor.layer = LayerMask.NameToLayer("Floor");
            floor.transform.SetParent(gameObject.transform);
        }
    }

    private bool CanMakeDoorway(int cellX, int cellY)
    {
        if (_dungeon[cellX, cellY] < (int)CellType.corridor)
            return false;
        
        int wallsAround = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                int x = cellX + j;
                int y = cellY + i;
                bool isMapEdge = x < 0 || y < 0 || x == _dungeon.Width || y == _dungeon.Height;

                if (isMapEdge || _dungeon[x, y] == (int)(CellType.wall))
                {
                    wallsAround++;
                }
            }
        }

        if (wallsAround >= 2 && wallsAround < 6)
        {
            bool wallToTheLeft = (cellX - 1 < 0 || _dungeon[cellX - 1, cellY] == (int)CellType.wall);
            bool wallToTheRight = (cellX + 1 == _dungeon.Width || _dungeon[cellX + 1, cellY] == (int)CellType.wall);

            bool wallAbove = (cellY - 1 < 0 || _dungeon[cellX, cellY - 1] == (int)CellType.wall);
            bool wallBelow = (cellY + 1 == _dungeon.Height || _dungeon[cellX, cellY + 1] == (int)CellType.wall);

            if ((wallAbove && wallBelow) | (wallToTheLeft && wallToTheRight))
            {
                return true;
            }
        }
        return false;
    }

    private int RotateDoorway(int x, int y, int xModifier, int yModifier)
    {
        int roomCellsCount = 0;

        for (int i = -1; i < 2; i++)
        {
            int newX = x + i * xModifier;
            int newY = y + i * yModifier;
            if ((yModifier == 1 && newY >= 0 && newY < _dungeon.Height)
                || (xModifier == 1 && newX >= 0 && newX < _dungeon.Width))
            {
                if (newX - yModifier >= 0 && newY - xModifier >= 0 && _dungeon[newX - yModifier, newY - xModifier] >= (int)CellType.room) //check left/above
                    roomCellsCount--;

                if (newX + yModifier < _dungeon.Width && newY + xModifier < _dungeon.Height && _dungeon[newX + yModifier, newY + xModifier] >= (int)CellType.room) //check right/below
                    roomCellsCount++;
            }
        }

        return roomCellsCount;
    }

    private void MakeWall(int cellX, int cellY)
    {
        int wallType = (int)CellType.wall;
        GameObject wall;
        Vector3 wallPosition = new Vector3(cellX*overallScale.x, 0, -cellY*overallScale.y); ;
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
                    if (x < 0 || y < 0 || y >= _dungeon.Height || x >= _dungeon.Width)
                        continue;

                    if(wallType != (_dungeon[x, y]))
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

            wall.transform.localScale = new Vector3(overallScale.x, overallScale.y + extraWallHeight, overallScale.z);
            wallPosition = new Vector3(cellX * overallScale.x + overallScale.x/2, 0, -cellY * overallScale.z - overallScale.z/2);
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

    private bool ShouldMakeWall(int cellX, int cellY, bool ignoreIsolatedWalls)
    {
        return (_dungeon[cellX, cellY] == (int)CellType.wall) && (!ignoreIsolatedWalls || !IsWallIsolated(cellX, cellY));
    }

    private bool IsWallIsolated(int cellX, int cellY)
    {
        for(int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (Mathf.Abs(i) == Mathf.Abs(j))
                    continue;
                
                int x = cellX + j;
                int y = cellY + i;

                if(x >= 0 && x < _dungeon.Width && y >= 0 && y < _dungeon.Height 
                    && _dungeon[x, y] != (int)CellType.wall)
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
        width *= overallScale.x;
        height *= overallScale.z;
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
