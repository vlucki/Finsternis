using System.Collections.Generic;
using UnityEngine;

public class Room : DungeonSection
{
    private List<Vector2> _cells;
    private MTRandom _random;

    public int CellCount { get { return _cells.Count; } }

    public Room(Vector2 position, MTRandom random) : base()
    {
        bounds = new Rect(position, Vector2.zero);
        _cells = new List<Vector2>();
        _random = random;
    }

    public Room(Room baseRoom) : base()
    {
        bounds = baseRoom.bounds;
        _cells = new List<Vector2>(baseRoom._cells);
        _random = baseRoom._random;
    }

    public static Room operator +(Room roomA, Room roomB)
    {
        Room mergedRooms = new Room(roomA);
        mergedRooms.Merge(roomB);

        return mergedRooms;
    }

    public void Merge(params Room[] others)
    {
        foreach (Room other in others)
        {
            foreach(Vector2 cell in other._cells)
                AddCell(cell);
            foreach(DungeonSection section in other.connections)
                AddConnection(section, true);

            bounds.min = Vector2.Min(bounds.min, other.bounds.min);
            bounds.max = Vector2.Max(bounds.max, other.bounds.max);
        }
    }

    public bool Overlaps(Room other)
    {
        if (this.bounds.Overlaps(other.bounds))
        {
            foreach(Vector2 cell in other)
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

    public void AddCell(Vector2 otherCell)
    {
        if (_cells.Count == 0)
            _cells.Add(otherCell);
        else
        {
            int[] result = Search(otherCell);
            if (result[0] >= 0)
                return;

            int l = result[1];

            if (l == _cells.Count)
                _cells.Add(otherCell);
            else
                _cells.Insert(l, otherCell);
        }

        AdjustSize(otherCell);
    }

    private void AdjustSize(Vector2 newCell)
    {
        bounds.min = Vector2.Min(bounds.min, newCell);
        bounds.max = Vector2.Max(bounds.max, newCell + Vector2.one);
    }

    public Vector2 GetRandomCell()
    {
        return _cells[_random.Range(0, _cells.Count, false)];
    }

    public bool ContainsCell(Vector2 cell)
    {
        return bounds.Contains(cell) && Contains(cell);
    }

    public override string ToString()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder("Room[bounds: ").Append(Bounds).Append("; cells: ").Append(_cells.ToString());
        return builder.ToString();
    }

    internal void Disconnect()
    {
        foreach(DungeonSection section in connections)
        {
            section.RemoveConnection(this);
        }
    }

    bool SearchCellsTouching(Room other)
    {
        foreach (Vector2 cell in other)
        {
            if(     this.ContainsCell(new Vector2(cell.x - 1, cell.y))
                ||  this.ContainsCell(new Vector2(cell.x + 1, cell.y))
                ||  this.ContainsCell(new Vector2(cell.x, cell.y - 1))
                ||  this.ContainsCell(new Vector2(cell.x, cell.y + 1)))
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

        if(     Pos.x <= roomB.bounds.xMax && bounds.xMax >= roomB.Pos.x
            &&  Pos.y <= roomB.bounds.yMax && bounds.yMax >= roomB.Pos.y)
            return SearchCellsTouching(roomB);

        return false;
    }

    public override IEnumerator<Vector2> GetEnumerator()
    {
        foreach (Vector2 cell in _cells)
            yield return cell;
    }
}