using System.Collections.Generic;
using UnityEngine;

namespace Finsternis
{
    public class Room : DungeonSection
    {
        private List<Vector2> _cells;
        private IRandom random;

        private bool _locked;

        public bool Locked { get { return _locked; } }

        public int CellCount { get { return _cells.Count; } }

        public static Room CreateInstance(Vector2 position, IRandom random)
        {
            Room r = CreateInstance<Room>(new Rect(position, Vector2.zero));
            r._cells = new List<Vector2>();
            r.random = random;
            return r;
        }

        public static Room CreateInstance(Rect bounds, IRandom random)
        {
            Room r = CreateInstance<Room>(bounds);
            r._cells = new List<Vector2>();
            r.random = random;
            return r;
        }
        public override string ToString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder("Room[bounds: ").Append(Bounds).Append("; cells: ").Append(_cells.ToString());
            return builder.ToString();
        }

        public override IEnumerator<Vector2> GetEnumerator()
        {
            foreach (Vector2 cell in _cells)
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
                foreach (Vector2 cell in other._cells)
                    AddCell(cell);
                foreach (DungeonSection section in other.connections)
                    AddConnection(section, true);

                bounds.min = Vector2.Min(bounds.min, other.bounds.min);
                bounds.max = Vector2.Max(bounds.max, other.bounds.max);
            }
        }

        public void Lock()
        {
            _locked = true;
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

        private bool Contains(Vector2 otherCell)
        {
            return Search(otherCell)[0] >= 0;
        }

        public int[] Search(Vector2 otherCell)
        {
            int[] result = { -1, -1 };

            if (_cells.Count > 0)
            {
                int l = 0;
                int r = _cells.Count;
                int m;

                do
                {
                    m = (l + r) / 2;
                    if (_cells[m] == otherCell)
                        result[0] = m;
                    else if (otherCell.y > _cells[m].y || (otherCell.y == _cells[m].y && otherCell.x > _cells[m].x))
                        l = m + 1;
                    else
                        r = m;
                } while (l < r && result[0] < 0);


                result[1] = l;
            }
            return result;
        }

        public void AddCell(float x, float y)
        {
            AddCell(new Vector2(x, y));
        }

        public override bool AddCell(Vector2 otherCell)
        {
            if (_cells.Count == 0)
                _cells.Add(otherCell);
            else
            {
                int[] result = Search(otherCell);
                if (result[0] >= 0)
                    return false;

                int l = result[1];

                if (l == _cells.Count)
                    _cells.Add(otherCell);
                else
                    _cells.Insert(l, otherCell);
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
                    Vector2 newMin = _cells[0];
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

        public Vector2 GetRandomCell()
        {
            return _cells[this.random.IntRange(0, _cells.Count, false)];
        }

        public override bool ContainsCell(Vector2 cell)
        {
            return bounds.Contains(cell) && Contains(cell);
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
                if  (  this.ContainsCell(new Vector2(cell.x - 1, cell.y))
                    || this.ContainsCell(new Vector2(cell.x + 1, cell.y))
                    || this.ContainsCell(new Vector2(cell.x, cell.y - 1))
                    || this.ContainsCell(new Vector2(cell.x, cell.y + 1))
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

            if (   Position.x <= roomB.bounds.xMax 
                && bounds.xMax >= roomB.Position.x
                && Position.y <= roomB.bounds.yMax 
                && bounds.yMax >= roomB.Position.y)
                return ContainsAdjacentCells(roomB);

            return false;
        }

        public override void RemoveCell(Vector2 cell)
        {
            _cells.Remove(cell);
            AdjustSize(cell, true);
        }
    }
}