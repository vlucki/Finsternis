using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityQuery;
using Random = UnityEngine.Random;

namespace Finsternis
{
    public class DungeonDrawer : MonoBehaviour
    {
        [Serializable]
        public class DrawingEndEvent : CustomEvent<Dungeon> { }

        [SerializeField]
        private Dungeon dungeon;

        [Header("Scaling parameters")]
        [Tooltip("Only affect walls and floors generated through primitives (not prefabs).")]
        public Vector3 cellScale = Vector3.one;
        
        public GameObject[] exits;

        [Header("Events")]
        public UnityEvent onDrawBegin;
        public DrawingEndEvent onDrawEnd;

        private HashSet<Vector2> drawnWalls;

        public Vector3 GetWorldPosition(Vector2 dungeonPosition)
        {
            return new Vector3(dungeonPosition.x * cellScale.x, 0, -dungeonPosition.y * cellScale.z);
        }

        public Vector2 GetDungeonPosition(Vector3 worldPosition)
        {
            return new Vector2((int)(worldPosition.x / cellScale.x), (int)(-worldPosition.z / cellScale.z));
        }

        public void Draw(Dungeon dungeon)
        {
            StopAllCoroutines();
            onDrawBegin.Invoke();
            this.dungeon = dungeon;
            Clear();

            if (!this.dungeon)
                throw new ArgumentException("Failed to find dungeon!");
            this.dungeon.gameObject.SetLayer("Ignore Raycast");
            var deathZoneBorders = this.dungeon.gameObject.AddComponent<BoxCollider>();
            deathZoneBorders.isTrigger = true;
            Vector3 dungeonCenter = GetWorldPosition(this.dungeon.GetCenter() + Vectors.Half2);

            deathZoneBorders.center = dungeonCenter;
            deathZoneBorders.size = new Vector3((this.dungeon.Width + 2) * this.cellScale.x, this.cellScale.y * 30, (this.dungeon.Height + 2) * this.cellScale.z);
            dungeon.gameObject.AddComponent<DeathZone>();

            this.drawnWalls = new HashSet<Vector2>();

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                var gen = _Draw(dungeon);
                while (gen.MoveNext())
                    ;
            }
            else
#endif
                StartCoroutine(_Draw(dungeon));
        }

        private IEnumerator _Draw(Dungeon dungeon)
        {
            GameObject rooms = new GameObject("ROOMS");
            rooms.transform.SetParent(this.dungeon.transform);

            foreach (Room room in dungeon.Rooms)
            {
                MakeSection(room).transform.SetParent(rooms.transform);
                yield return null;
            }

            GameObject corridors = new GameObject("CORRIDORS");
            corridors.transform.SetParent(dungeon.transform);

            foreach (Corridor corridor in this.dungeon.Corridors)
            {
                MakeSection(corridor).transform.SetParent(corridors.transform);
                yield return null;
            }


            onDrawEnd.Invoke(dungeon);
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

                        var wall = MakeWall(cell, new Vector2(i, j));
                        if (wall)
                            wall.transform.SetParent(sectionGO.transform);
                    }

                }
                GameObject sectionCell = MakeCell((int)cell.x, (int)cell.y);
                sectionCell.transform.SetParent(sectionGO.transform);
            }

            return sectionGO;
        }

        private GameObject MakeWall(Vector2 cell, Vector2 wallOffset)
        {
            var wallPos = cell + wallOffset;
            if (ShouldMakeWall(wallPos))
            {
                try
                {
                    var theme = dungeon[cell].Theme;
                    theme.GetRandomWall();
                    var wall = (GameObject)Instantiate(theme.GetRandomWall().GetLateral(), GetWorldPosition(wallPos + Vectors.Half2), Quaternion.identity);
                    wall.transform.forward = new Vector3(-wallOffset.x, 0, wallOffset.y);
                    return wall;
                }
                catch (NullReferenceException ex)
                {
#if DEBUG
                    string msg = "{0}\nFailed to creat wall for cell {1}\n";
                    if (!dungeon[cell])
                        msg += "Position was null";
                    else if (dungeon[cell].Theme == null)
                        msg += "No theme set for " + dungeon[cell];
                    Log.E(this, msg, ex, cell.ToString("0"));
#endif
                }
                catch (IndexOutOfRangeException ex)
                {
#if DEBUG
                    string msg = "{0}\nFailed to creat wall for cell {1}\nPosition was out of bounds";
                    Log.E(this, msg, ex, cell.ToString("0"));
#endif
                }
            }

            return null;
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
            Vector2 dungeonPos = new Vector2(cellX, cellY);
            Vector3 worldPos = GetWorldPosition(dungeonPos + Vectors.Half2);

            string name = "floor (" + cellX + ";" + cellY + ")";

            GameObject cell = new GameObject(name);
            cell.transform.position = worldPos;
            cell.layer = LayerMask.NameToLayer("Floor");
            cell.transform.SetParent(dungeon.transform);

            DungeonFeature replacement = null;
            var featuresList = dungeon.GetFeaturesAt(dungeonPos);
            if (featuresList != null)
            {
                int count = 1;
                DungeonFeature lastFeature = null;
                foreach (var feature in featuresList)
                {
                    if (feature != lastFeature)
                    {
                        lastFeature = feature;
                        count = 1;
                    }
                    if (feature.Type == DungeonFeature.FeatureType.REPLACEMENT)
                        replacement = feature;
                    MakeFeature(feature, new Vector2(cellX, cellY), count).transform.SetParent(cell.transform);
                    count++;
                }
            }

            if (!replacement)
            {
                GameObject floor;
                try
                {
                    floor = cell.AddChild(dungeon[dungeonPos].Theme.GetRandomFloor());
                }
                catch (IndexOutOfRangeException ex)
                {
                    floor = cell.AddChild(GameObject.CreatePrimitive(PrimitiveType.Quad));
                    floor.transform.rotation = Quaternion.Euler(90, 0, 0);
#if DEBUG
                    string msg = "{0}\nFailed to creat wall for cell {1}\nPosition was out of bounds";
                    Log.E(this, msg, ex, dungeonPos.ToString("0"));
#endif
                }

                floor.transform.Rotate(Vector3.up, Random.Range(0, 4) * 90, Space.World);
            }

            return cell;
        }

        private GameObject MakeFeature(DungeonFeature feature, Vector2 position, int count)
        {
            GameObject featureGO =
                (GameObject)Instantiate(
                feature.Prefab,
                GetWorldPosition(position + Vectors.Half2),
                feature.Prefab.transform.rotation);
            if (!feature.Alignment.IsNullOrEmpty())
            {
                feature.Alignment.ForEach(
                    alignment => { if (alignment) alignment.Align(this.dungeon, this.cellScale, position, featureGO, count); });
            }

            return featureGO;
        }

        private GameObject MakeWall(int cellX, int cellY)
        {
            var dungeonPos = new Vector2(cellX, cellY);

            if (drawnWalls.Contains(dungeonPos) || (dungeon.IsWithinDungeon(dungeonPos) && !dungeon.IsOfType(dungeonPos, null)))
                return null;

            drawnWalls.Add(dungeonPos);

            GameObject wall = new GameObject("Wall (" + dungeonPos.ToString("0") + ")");
            wall.transform.position = GetWorldPosition(dungeonPos + Vectors.Half2);
            wall.layer = LayerMask.NameToLayer("Wall");

            return wall;
        }

        /// <summary>
        /// Checks if a wall should be made at a given position in the dungeon.
        /// </summary>
        /// <param name="pos">Position to check.</param>
        /// <returns>True if the given coordinates are outside the dungeon or represent a wall within it.</returns>
        public bool ShouldMakeWall(Vector2 pos)
        {
            return (!dungeon.IsWithinDungeon(pos) || !dungeon[pos]);
        }

        /// <summary>
        /// Checks if a wall should be made at a given position in the dungeon.
        /// </summary>
        /// <param name="cellX">Column being checked.</param>
        /// <param name="cellY">Row being checked.</param>
        /// <returns>True if the given coordinates are outside the dungeon or represent a wall within it.</returns>
        private bool ShouldMakeWall(int cellX, int cellY)
        {
            return (!dungeon.IsWithinDungeon(cellX, cellY) || !dungeon[cellX, cellY]);
        }

        public Vector3 GetEntrancePosition()
        {
            return GetWorldPosition(this.dungeon.Entrance + Vectors.Half2);
        }
    }

}