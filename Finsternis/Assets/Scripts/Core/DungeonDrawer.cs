using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DungeonDrawer : MonoBehaviour
{
    public int mergeThreshold = 1000;
    private GameObject floor;

    public void DrawFromGrid<T>(T[,] grid, T wall)
    {
        Clear();

        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        MakeFloor(width, height);
        string gridString = "";

        for (int cellY = 0; cellY < height; cellY++)
        {
            string row = "|";
            for (int cellX = 0; cellX < width; cellX++)
            {
                row += grid[cellX, cellY] + "|";
                if (ShouldMakeWall<T>(grid, cellX, cellY, wall))
                {
                    MakeWall(cellX, cellY);
                }
            }
            gridString += row + "\n";
        }

        Debug.Log(gridString);
        MergeMeshes();
    }

    private void MakeWall(int cellX, int cellY)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.SetParent(gameObject.transform);
        wall.transform.position = new Vector3(cellX + 0.5f, 0.5f, - cellY - 0.5f);
    }

    private bool ShouldMakeWall<T>(T[,] grid, int cellX, int cellY, T wall)
    {
        return EqualityComparer<T>.Default.Equals(grid[cellX, cellY], wall);
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
                combine = new CombineInstance[Math.Min(meshFilters.Length - i, mergeThreshold)];
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
            catch (IndexOutOfRangeException ex)
            {
                Debug.Log(ex);
                Debug.Log(j);
                Debug.Log(i);
                Debug.Log(i - j * meshFilters.Length);
                Debug.Log(meshFilters.Length);
                Debug.Log(combine.Length);
                throw new Exception();
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
        Shader s = Shader.Find("Custom/myShader");
        Material mat = new Material(s);
        r.material = mat;

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


    private void MakeFloor(int width, int height)
    {
        floor = GameObject.CreatePrimitive(PrimitiveType.Quad);
        floor.name = "floor";
        floor.transform.localScale = new Vector3(width, height, 1);
        floor.transform.position = new Vector3(width / 2, 0f, - height / 2);
        floor.GetComponent<MeshRenderer>().sharedMaterial.color = Color.green;
        floor.transform.SetParent(gameObject.transform);
        floor.transform.up = Vector3.forward;
    }
}
