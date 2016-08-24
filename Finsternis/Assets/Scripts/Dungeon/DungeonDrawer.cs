using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityQuery;
using Random = UnityEngine.Random;

namespace Finsternis
{
    public class DungeonDrawer : MonoBehaviour
    {
        [SerializeField]
        private Dungeon dungeon;

        [Header("Scaling parameters")]
        [Tooltip("Only affect walls and floors generated through primitives (not prefabs).")]
        public Vector3 cellScale = Vector3.one;
        public float extraWallHeight = 3;

        [Header("Materials")]
        public Material defaultWallMaterial;
        public PhysicMaterial defaultWallPhysicMaterial;
        public Material defaultFloorMaterial;
        public PhysicMaterial defaultFloorPhysicMaterial;
        public Material corridorMaterial;

        [Header("Prefabs")]
        public GameObject[] walls;

        public GameObject[] floorTiles;
        public GameObject[] exits;

        [Header("Events")]
        public UnityEvent onDrawBegin;
        public UnityEvent onDrawEnd;

        private HashSet<Vector2> drawnWalls;

        public Vector3 GetWorldPosition(Vector2 dungeonPosition)
        {
            return new Vector3(dungeonPosition.x * cellScale.x, 0, -dungeonPosition.y * cellScale.z);
        }

        public void Draw(Dungeon dungeon)
        {
            onDrawBegin.Invoke();
            this.dungeon = dungeon;
            Clear();

            if (!this.dungeon)
                throw new ArgumentException("Failed to find dungeon!");

            this.drawnWalls = new HashSet<Vector2>();
            GameObject rooms = new GameObject("ROOMS");
            rooms.transform.SetParent(this.dungeon.transform);

            foreach (Room room in this.dungeon.Rooms)
            {
                MakeSection(room).transform.SetParent(rooms.transform);
            }

            GameObject corridors = new GameObject("CORRIDORS");
            corridors.transform.SetParent(dungeon.transform);

            foreach (Corridor corridor in this.dungeon.Corridors)
            {
                MakeSection(corridor).transform.SetParent(corridors.transform);
            }

            //MakeWalls();

            onDrawEnd.Invoke();
        }

        private void Clear()
        {
            if (!dungeon)
                return;
            dungeon.gameObject.DestroyChildren();
        }

        private GameObject MakeSection(DungeonSection section)
        {
            Type type = section.GetType();
            GameObject sectionGO = new GameObject(type.ToString() + " " + section.Position.ToString("F0"));
            sectionGO.transform.position = GetWorldPosition(section.Bounds.center);
            foreach (Vector2 cell in section)
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (Mathf.Abs(i) == Mathf.Abs(j))
                            continue;

                        GameObject wall = MakeWall((int)cell.x + i, (int)cell.y + j);
                        if (wall)
                            wall.transform.SetParent(sectionGO.transform);
                    }
                }
                GameObject sectionCell = MakeCell((int)cell.x, (int)cell.y);
                sectionCell.transform.SetParent(sectionGO.transform);
            }

            return sectionGO;
        }

        private void MakeWalls()
        {
            GameObject wallsContainer = new GameObject("WALLS");
            wallsContainer.transform.SetParent(dungeon.transform);
            int width = dungeon.Width;
            int height = dungeon.Height;
            for (int cellY = -1; cellY <= height; cellY++)
            {
                for (int cellX = -1; cellX <= width; cellX++)
                {
                    if (ShouldMakeWall(cellX, cellY))
                    {
                        GameObject wall = MakeWall(cellX, cellY);
                        bool mayBlockPlayer = ShouldAddScript(cellX, cellY);
                        if (mayBlockPlayer
                            || ShouldAddScript(cellX, cellY - 1)
                            || ShouldAddScript(cellX - 1, cellY - 1)
                            || ShouldAddScript(cellX + 1, cellY - 1))
                        {
                            if (wall.GetComponent<Renderer>())
                            {
                                Wall w = wall.AddComponent<Wall>();
                                w.canFadeCompletely = mayBlockPlayer;
                            }
                        }
                        wall.transform.SetParent(wallsContainer.transform);
                    }
                }
            }
        }

        private bool ShouldAddScript(int cellX, int cellY)
        {
            return (cellX >= 0
                && cellX < dungeon.Width
                && cellY > 0
                && dungeon[cellX, cellY - 1] != null);
        }

        private GameObject MakeCell(int cellX, int cellY)
        {
            GameObject cell = null;
            Vector3 pos = GetWorldPosition(new Vector2(cellX, cellY) + Vector2.one / 2);

            string name = "floor (" + cellX + ";" + cellY + ")";
            DungeonFeature feature = dungeon[cellX, cellY].GetFeature(cellX, cellY);
            if (!feature || feature.Type != DungeonFeature.FeatureType.REPLACEMENT)
            {
                cell = MakeQuad(pos,
                                new Vector3(cellScale.x, cellScale.z, 1),
                                Vector3.right * 90,
                                dungeon.IsOfType(cellX, cellY, typeof(Corridor)) ? corridorMaterial : defaultFloorMaterial,
                                name);

                if (cellX == (int)dungeon.Exit.x && cellY == (int)dungeon.Exit.y)
                {
                    if (exits != null && exits.Length > 0)
                    {
                        ClearObject(cell);
                        cell = Instantiate(exits[Random.Range(0, exits.Length)], pos, Quaternion.identity) as GameObject;
                    }
                    else
                    {
                        cell.name = "Exit " + name;
                        cell.tag = "Exit";
                        cell.AddComponent<Exit>();
                    }
                }
                else
                {
                    if (floorTiles != null && floorTiles.Length > 0 && Random.value > 0.5f)
                    {
                        cell = Instantiate(floorTiles[Random.Range(0, floorTiles.Length)], pos, Quaternion.Euler(0, 90 * Random.Range(0, 4), 0)) as GameObject;
                        cell.transform.localScale = cellScale;
                    }
                    if (cellX == (int)dungeon.Entrance.x && cellY == (int)dungeon.Entrance.y)
                    {
                        GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        pedestal.name = "Entrance";
                        pedestal.transform.SetParent(cell.transform);
                        pedestal.transform.localScale = new Vector3(1, 0.05f, 1);
                        pedestal.transform.localPosition = Vector3.zero;
                        ClearObject(pedestal.GetComponent<CapsuleCollider>());
                    }
                }
                if (feature)
                    MakeFeature(feature, new Vector3(cellX, cellY)).transform.SetParent(cell.transform);
            }
            else
            {
                try
                {
                    cell = Instantiate(feature.Prefab);

                    if (dungeon[cellX, cellY] is Corridor)
                    {
                        Vector2 corridorDir = ((Corridor)dungeon[cellX, cellY]).Direction;
                        cell.transform.forward = new Vector3(corridorDir.y, 0, corridorDir.x);
                    }
                    cell.transform.position = pos;
                }
                catch (IndexOutOfRangeException ex)
                {
                    throw new IndexOutOfRangeException("Trying to access cell (" + cellX + "; " + cellY + ")", ex);
                }
            }
            if (cell)
            {
                cell.layer = LayerMask.NameToLayer("Floor");
                cell.transform.SetParent(dungeon.transform);
            }

            return cell;
        }

        private GameObject MakeFeature(DungeonFeature feature, Vector2 position)
        {
            position += Vector2.one / 2; //needed to align the feature with the center of each cell

            GameObject featureGO =
                (GameObject)Instantiate(
                feature.Prefab,
                GetWorldPosition(position) + feature.Offset,
                Quaternion.identity);

            switch (feature.Alignment)
            {
                case DungeonFeature.CellAlignment.FLOOR:

                    if (!feature.Offset.IsZero())
                        featureGO.transform.forward = feature.Offset;

                    break;

                case DungeonFeature.CellAlignment.WALL:

                    if (dungeon.IsOfType(position + Vector2.up, null))
                        featureGO.transform.up = Vector3.forward;   //wall is "above"
                    else if (dungeon.IsOfType(position + Vector2.down, null))
                        featureGO.transform.up = Vector3.back;      //wall is "below"
                    else if (dungeon.IsOfType(position + Vector2.right, null))
                        featureGO.transform.up = Vector3.right;     //wall is to the right
                    else if (dungeon.IsOfType(position + Vector2.left, null))
                        featureGO.transform.up = Vector3.left;      //wall is to the left

                    break;
            }

            return featureGO;
        }

        private GameObject MakeWall(int cellX, int cellY)
        {
            var coords = new Vector2(cellX, cellY);

            if (drawnWalls.Contains(coords) || (dungeon.IsWithinDungeon(coords) && !dungeon.IsOfType(cellX, cellY, null)))
                return null;

            drawnWalls.Add(coords);

            GameObject wall;
            Vector3 wallPosition = GetWorldPosition(coords);

            if (walls != null && walls.Length > 0)
            {
                wall = walls[Random.Range(0, walls.Length)];
            }
            else
            {
                wall = new GameObject("Wall (" + cellX + ";" + cellY + ")");
                //GameObject top = MakeQuad(Vector3.up, Vector3.one, Vector3.right * 90, defaultWallMaterial, "WallTop");
                //top.transform.SetParent(wall.transform);

                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (Mathf.Abs(i) == Mathf.Abs(j))
                            continue;
                        int x = cellX + j;
                        int y = cellY + i;
                        if (!dungeon.IsWithinDungeon(x, y))
                            continue;

                        if (dungeon[x, y] != null)
                        {
                            var offset = Vector2.down;
                            float angle = 0;
                            if (x < cellX)
                            {
                                offset = Vector2.right;
                                angle = 90;
                            }
                            else if (x > cellX)
                            {
                                offset = Vector2.left;
                                angle = -90;
                            }
                            else if (y < cellY)
                            {
                                offset = Vector2.up;
                                angle = 180; //make wall face away the camera
                            }

                            float angleB = offset.y <= 0 ? -angle : 0;
                            GameObject sideA = MakeQuad(new Vector3((float)j / 2, 0.5f, -(float)i / 2), Vector3.one, new Vector3(0, angle, 0), defaultWallMaterial, "WallSideA");
                            GameObject sideB = MakeQuad(new Vector3((float)j / 2 + offset.x / 50, 0.5f, -(float)i / 2 - offset.y / 50), Vector3.one, new Vector3(0, angleB, 0), defaultWallMaterial, "WallSideB");

                            sideA.GetComponent<Collider>().sharedMaterial = defaultWallPhysicMaterial;
                            sideA.GetComponent<MeshRenderer>().sharedMaterial = defaultWallMaterial;
                            sideA.transform.SetParent(wall.transform);

                            ClearObject(sideB.GetComponent<Collider>());
                            sideB.GetComponent<MeshRenderer>().sharedMaterial = defaultWallMaterial;
                            sideB.transform.SetParent(wall.transform);
                        }
                    }
                }

                wall.transform.localScale = new Vector3(cellScale.x, cellScale.y + extraWallHeight, cellScale.z);
                wallPosition = GetWorldPosition(coords + Vector2.one / 2);
            }

            wall.transform.position = wallPosition;
            wall.layer = LayerMask.NameToLayer("Wall");
            return MergeMeshes(wall, true)[0];
        }

        /// <summary>
        /// Creates a game object with a primitive quad mesh.
        /// </summary>
        /// <param name="pos">Position where the quad should be.</param>
        /// <param name="scale">Scale of the resulting quad.</param>
        /// <param name="rotation">Rotation of the resulting quad.</param>
        /// <param name="mat">Material to be used on the resulting quad.</param>
        /// <param name="name">Name of the resulting gameobject.</param>
        /// <returns></returns>
        private GameObject MakeQuad(Vector3 pos, Vector3 scale, Vector3 rotation, Material mat, string name = "Ceiling")
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Quad);
            plane.GetComponent<MeshRenderer>().sharedMaterial = mat;
            plane.transform.localScale = scale;
            plane.transform.rotation = Quaternion.Euler(rotation);
            plane.transform.position = pos;
            plane.name = name;
            return plane;
        }

        /// <summary>
        /// Checks if a wall should be made at a given position in the dungeon.
        /// </summary>
        /// <param name="cellX">Column being checked.</param>
        /// <param name="cellY">Row being checked.</param>
        /// <returns>True if the given coordinates are outside the dungeon or represent a wall within it.</returns>
        private bool ShouldMakeWall(int cellX, int cellY)
        {
            return (!dungeon.IsWithinDungeon(cellX, cellY) || dungeon[cellX, cellY] == null);
        }

        /// <summary>
        /// Merge every mesh within a game object and its children.
        /// </summary>
        /// <param name="parent">Game object containing the mesh and/or children with meshes.</param>
        /// <param name="nameAfterParent">Should the merged mesh have the same name as the parent mesh?</param>
        /// <param name="mergeThreshold">How many meshes may be combined at once?</param>
        /// <returns></returns>
        private GameObject[] MergeMeshes(GameObject parent, bool nameAfterParent = false, int mergeThreshold = 5)
        {
            Vector3 originalPos = parent.transform.position;
            parent.transform.position = Vector3.zero;
            List<GameObject> sections = new List<GameObject>(1);
            MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
            if (meshFilters.Length < 1)
            {
                sections.Add(parent);
                return sections.ToArray();
            }

            CombineInstance[] combine = new CombineInstance[System.Math.Min(meshFilters.Length, mergeThreshold)];
            bool combined = false;
            int i = 0;
            int j = 0;
            while (i < meshFilters.Length)
            {
                if (j >= combine.Length)
                {
                    sections.Add(CreateSection(j, combine, parent, nameAfterParent ? parent.name : null));
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
                catch (IndexOutOfRangeException ex)
                {
                    Debug.Log(
                        "j = " + j +
                        "\ni = " + i +
                        "\nmeshFilters.Length = " + meshFilters.Length +
                        "\ncombine.Length = " + combine.Length
                        );
                    throw new IndexOutOfRangeException(ex.Message);
                }
                i++;
            }

            if (!combined)
            {
                sections.Add(CreateSection(j, combine, parent, nameAfterParent ? parent.name : null));
            }

            foreach (MeshFilter m in meshFilters)
            {
                ClearObject(m.gameObject);
            }

            ClearObject(parent);

            foreach (GameObject section in sections)
                section.transform.Translate(originalPos);

            return sections.ToArray();
        }

        private GameObject CreateSection(int j, CombineInstance[] combine, GameObject original, string name = null, bool useBoxCollider = true)
        {
            if (String.IsNullOrEmpty(name))
                name = "section" + j;

            GameObject sectionContainer = new GameObject(name);

            Mesh m = new Mesh();
            m.CombineMeshes(combine);
            sectionContainer.AddComponent<MeshFilter>().mesh = m;

            MeshRenderer r = sectionContainer.GetComponent<MeshRenderer>();
            if (!r)
                r = sectionContainer.AddComponent<MeshRenderer>();

            r.material = Material.Instantiate<Material>(defaultWallMaterial);

            if (useBoxCollider)
                sectionContainer.AddComponent<BoxCollider>().sharedMaterial = defaultWallPhysicMaterial;
            else
                sectionContainer.AddComponent<MeshCollider>().sharedMaterial = defaultWallPhysicMaterial;

            sectionContainer.layer = original.layer;
            return sectionContainer;
        }

        private void ClearObject(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                DestroyImmediate(obj);
            else
                Destroy(obj);
#else
        Destroy(obj);
#endif
        }
    }
}