namespace Finsternis
{
    using System.Collections.Generic;
    using UnityEngine;
    using Extensions;

    public class Room : DungeonSection
    {
        private List<Vector2> cells;

        private bool locked;

        public bool Locked { get { return this.locked; } }

        public int CellCount { get { return this.cells.Count; } }

        public new RoomTheme Theme { get { return base.Theme as RoomTheme; } }

        public static Room CreateInstance(Vector2 position, Dungeon dungeon)
        {
            Room r = CreateInstance<Room>(new Rect(position, Vector2.zero), dungeon);
            r.cells = new List<Vector2>();
            return r;
        }

        public override string ToString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder("Room[bounds: ").Append(Bounds).Append("; cells: ").Append(this.cells.ToString());
            return builder.ToString();
        }

        public override IEnumerator<Vector2> GetEnumerator()
        {
            foreach (Vector2 cell in this.cells)
                yield return cell;
        }

        public static Room operator +(Room roomA, Room roomB)
        {
            Room mergedRooms = Instantiate(roomA);
            mergedRooms.Merge(roomB);

            return mergedRooms;
        }

        public void Merge(params Room[] others)
        {
            foreach (Room other in others)
            {
                foreach (Vector2 cell in other.cells)
                    AddCell(cell);
                foreach (DungeonSection section in other.connections)
                    AddConnection(section, true);

                bounds.min = Vector2.Min(bounds.min, other.bounds.min);
                bounds.max = Vector2.Max(bounds.max, other.bounds.max);
            }
        }

        public void Lock()
        {
            this.locked = true;
        }

        public bool Overlaps(Room other)
        {
            if (this.bounds.Overlaps(other.bounds))
            {
                foreach (Vector2 cell in other)
                {
                    if (this.Contains(cell))
                        return true;
                }
            }
            return false;
        }

        public override void SetTheme<T>(T theme)
        {
            if (!(theme is RoomTheme))
                return;
            base.SetTheme<T>(theme);
        }

        public void SetTheme(RoomTheme theme)
        {
            this.SetTheme<RoomTheme>(theme);
        }

        /// <summary>
        /// Looks for a given cell within the room using a binary search
        /// </summary>
        /// <param name="otherCell">The cell being searched.</param>
        /// <returns></returns>
        public int[] Search(Vector2 otherCell)
        {
            int[] result = { -1, -1 };

            if (this.cells.Count > 0)
            {
                int leftBound = 0;
                int rightBound = this.cells.Count;
                int middle;

                do
                {
                    middle = (leftBound + rightBound) / 2;
                    Vector2 middleCell = this.cells[middle];
                    if (this.cells[middle] == otherCell)
                        result[0] = middle;
                    else if (otherCell.y > middleCell.y || (otherCell.y == middleCell.y && otherCell.x > middleCell.x))
                        leftBound = middle + 1;
                    else
                        rightBound = middle;
                } while (leftBound < rightBound && result[0] < 0);


                result[1] = leftBound;
            }
            return result;
        }

        public void AddCell(float x, float y)
        {
            AddCell(new Vector2(x, y));
        }

        public override bool AddCell(Vector2 otherCell)
        {
            if (this.cells.Count == 0)
                this.cells.Add(otherCell);
            else
            {
                int[] result = Search(otherCell);
                if (result[0] >= 0)
                    return false;

                int l = result[1];

                if (l == this.cells.Count)
                    this.cells.Add(otherCell);
                else
                    this.cells.Insert(l, otherCell);
            }

            AdjustSize(otherCell);

            return true;
        }

        /// <summary>
        /// Ensures the room bounds are consitent after adding or removing a given cell.
        /// </summary>
        /// <param name="cell">The cell that was added or removed.</param>
        /// <param name="cellRemoved">What happened to the cell.</param>
        private void AdjustSize(Vector2 cell, bool cellRemoved = false)
        {
            if (cellRemoved) //if the cell was removed
            {
                //if this cell was at the edge of the room
                if (bounds.min.x == cell.x || bounds.min.y == cell.y || bounds.max.x == cell.x || bounds.max.y == cell.y)
                {
                    Vector2 newMin = this.cells[0];
                    Vector2 newMax = newMin;

                    //iterate through the remaining cells and define the new bounds, if needed
                    foreach (Vector2 existingCell in this)
                    {
                        newMin = Vector2.Min(newMin, existingCell);
                        newMax = Vector2.Max(newMax, existingCell);
                    }
                }
            }
            else //if not, just check strech the bounds to accommodate it, if needed
            {
                bounds.min = Vector2.Min(bounds.min, cell);
                bounds.max = Vector2.Max(bounds.max, cell + Vector2.one);
            }
        }

        public override Vector2 GetRandomCell(params int[] constraints)
        {
            return this.cells.GetRandom(UnityEngine.Random.Range);
        }

        public override bool Contains(Vector2 cell)
        {
            return bounds.Contains(cell) && Search(cell)[0] >= 0;
        }

        internal void Disconnect()
        {
            foreach (DungeonSection section in connections)
            {
                section.RemoveConnection(this);
            }
        }

        public bool ContainsAdjacentCells(Room other)
        {
            foreach (Vector2 cell in other)
            {
                if (this.Contains(new Vector2(cell.x - 1, cell.y))
                    || this.Contains(new Vector2(cell.x + 1, cell.y))
                    || this.Contains(new Vector2(cell.x, cell.y - 1))
                    || this.Contains(new Vector2(cell.x, cell.y + 1))
                    )
                {
                    return true;
                }
            }
            return false;
        }

        internal bool IsTouching(Room roomB)
        {
            if (this.Overlaps(roomB))
                return true;

            if (Position.x <= roomB.bounds.xMax
                && bounds.xMax >= roomB.Position.x
                && Position.y <= roomB.bounds.yMax
                && bounds.yMax >= roomB.Position.y)
                return ContainsAdjacentCells(roomB);

            return false;
        }

        public override void RemoveCell(Vector2 cell)
        {
            this.cells.Remove(cell);
            AdjustSize(cell, true);
        }
    }
}