using System.Collections.Generic;
using UnityEngine;

public class Room : DungeonSection
{
    private List<Vector2> _cells;

    public List<Vector2> Cells { get { return _cells; } }

    public Room(Vector2 position)
    {
        _bounds = new Rect(position, Vector2.zero);
        _cells = new List<Vector2>();
    }

    private void AdjustSize(Vector2 pos)
    {
        if (pos.x < _bounds.x)
            _bounds.x = pos.x;
        else if (pos.x > _bounds.xMax)
            _bounds.xMax = pos.x;

        if (pos.y < _bounds.y)
            _bounds.y = pos.y;
        else if (pos.y > _bounds.yMax)
            _bounds.yMax = pos.y;
    }

    public void Merge(Room other)
    {
        _cells.AddRange(other._cells);
        _bounds.xMin = Mathf.Min(_bounds.xMin, other._bounds.xMin);
        _bounds.yMin = Mathf.Min(_bounds.yMin, other._bounds.yMin);
        _bounds.xMax = Mathf.Max(_bounds.xMax, other._bounds.xMax);
        _bounds.yMax = Mathf.Max(_bounds.yMax, other._bounds.yMax);
    }

    public bool Intersects(Room other)
    {
        if (other._bounds.Overlaps(other._bounds))
        {
            foreach(Vector2 cell in other._cells)
            {
                if (_cells.Contains(cell))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void AddCell(float x, float y)
    {
        AddCell(new Vector2(x, y));
    }

    public void AddCell(Vector2 pos)
    {
        _cells.Add(pos);
        AdjustSize(pos);
    }

    public Vector2 GetRandomCell()
    {
        return _cells[Random.Range(0, _cells.Count)];
    }

    public override string ToString()
    {
        System.Text.StringBuilder cells = new System.Text.StringBuilder("Room[bounds: ").Append(Bounds).Append("; cells: ");
        _cells.ForEach(cell => cells.Append(cell).Append("; "));
        return cells.ToString(0, cells.Length - 2);
    }

}