using UnityEngine;
using System.Collections.Generic;

public class DungeonDrawer : MonoBehaviour
{
    public int mergeThreshold = 1000;
    public Vector3 scale = Vector3.one;
    public Material defaultWallMaterial;
    public PhysicMaterial defaultWallPhysicMaterial;
    public Material defaultFloorMaterial;
    public PhysicMaterial defaultFloorPhysicMaterial;

    public GameObject[] walls;

    private GameObject _floor;

    public void DrawFromGrid<T>(T[,] grid, T wall, bool ignoreIsolatedWalls = true)
    {
        Clear();

        int width = grid.GetLength(1);
        int height = grid.GetLength(0);

        MakeFloor(width, height);

        for (int cellY = -1; cellY <= height; cellY++)
        {
            for (int cellX = -1; cellX <= width; cellX++)
            {
                if (cellY < 0 || cellY == height || cellX < 0 || cellX == width || ShouldMakeWall<T>(grid, cellX, cellY, wall, ignoreIsolatedWalls))
                {
                    MakeWall(cellX, cellY);
                }
                else if(ShouldMakeWall<T>(grid, cellX, cellY, wall, false))
                {
                    MakeCeiling(cellX, cellY);
                }
            }
        }

        //MergeMeshes();
    }

    private void MakeWall(int cellX, int cellY)
    {
        GameObject wall;
        Vector3 wallPosition = new Vector3(cellX*scale.x, 0, -cellY*scale.y); ;
        if (walls != null && walls.Length > 0)
        {
            wall = walls[Random.Range(0, walls.Length)];
        }
        else
        {
            wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.GetComponent<MeshRenderer>().sharedMaterial = defaultWallMaterial;
            wall.GetComponent<BoxCollider>().sharedMaterial = defaultWallPhysicMaterial;
            wallPosition = new Vector3(cellX*scale.x + scale.x / 2, scale.y / 2, -cellY*scale.z - scale.z / 2);
            wall.name = "Wall";
        }
        wall.transform.localScale = scale;
        wall.transform.position = wallPosition;
        wall.transform.SetParent(gameObject.transform);
    }

    private void MakeCeiling(int cellX, int cellY)
    {
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Quad);
        ceiling.GetComponent<MeshRenderer>().sharedMaterial = defaultWallMaterial;
        ceiling.transform.localScale = new Vector3(scale.x, scale.z, 1);
        ceiling.transform.Rotate(Vector3.right * 90);
        ceiling.transform.position = new Vector3(cellX * scale.x + scale.x / 2, scale.y, -cellY * scale.z - scale.z / 2);
        ceiling.transform.SetParent(gameObject.transform);
        ceiling.name = "Ceiling";
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

    private void MergeMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
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
            AddSection(j, combine);
        }

        foreach (MeshFilter m in meshFilters)
            DestroyImmediate(m.gameObject);

    }

    private void AddSection(int j, CombineInstance[] combine)
    {
        GameObject sectionContainer = new GameObject("section" + j);
        sectionContainer.transform.SetParent(transform);
        Mesh m = new Mesh();
        m.CombineMeshes(combine);
        sectionContainer.AddComponent<MeshFilter>().mesh = m;
        MeshRenderer r = sectionContainer.GetComponent<MeshRenderer>();
        if (!r)
            r = sectionContainer.AddComponent<MeshRenderer>();
        //Shader s = Shader.Find("Custom/myShader");
        //Material mat = new Material(s);
        r.material = defaultWallMaterial;

        sectionContainer.AddComponent<MeshCollider>();
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
