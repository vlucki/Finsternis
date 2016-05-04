using UnityEngine;
public class Corridor : DungeonSection
{
    private Vector2 _direction;
    private int _length;
    //private DungeonSection[] _connections;

    public Vector2 Direction { get { return _direction; } set { _direction = value; } }
    public override Rect Bounds
    {
        get { return bounds; }
        set
        {
            bounds = value;
            _length = Mathf.RoundToInt(Mathf.Max(bounds.size.x, bounds.size.y));
        }
    }

    public int Length
    {
        get { return _length; }

        set
        {
            if (value > 0)
            {
                _length = value;
                bounds.size = new Vector2(value * _direction.x + _direction.y, value * _direction.y + _direction.x);
            }
            else
            {
                _length = 0;
                bounds.size = Vector2.zero;
            }
        }
    }

    public Vector2 LastCell
    {
        get { return _length > 0 ? this[Length-1] : Pos; }
    }

    public Vector2 this[int index]
    {
        get
        {
            if (index < 0 || index >= _length)
                throw new System.ArgumentOutOfRangeException("index", "Trying to access a cell with index " + index + " within " + ToString());
            return bounds.position + _direction * index;
        }
    }

    public Corridor(Rect bounds, Vector2 direction) : base()
    {
        _direction = direction;
        Bounds = bounds;
        //_connections = new DungeonSection[2];
    }

    public Corridor(Vector2 position, int length, Vector2 direction) : 
        this(new Rect(position, direction * length + new Vector2(direction.y, direction.x)), direction)
    {
    }

    public void UpdateConnections()
    {
        //if (_connections[0] != null)
        //    if (_connections[0].GetType().Equals(typeof(Corridor)))
        //        ((Corridor)_connections[0]).AddConnection(this, Pos);
        //    else
        //        ((Room)_connections[0]).AddConnection(this);

    }

    //private void UpdateConnection(int index = 0)
    //{
    //    if (_connections[index].GetType().Equals(typeof(Corridor)))
    //        ((Corridor)_connections[index]).AddConnection(this, index == 0 ? Pos : this[Length-1]);
    //    else
    //        ((Room)_connections[index]).AddConnection(this);
    //}

    //public void AddConnection(DungeonSection section, Vector2 position)
    //{
    //    bool atStart = (position.x < Pos.x && Direction.x == 1) || (position.y < Pos.y && Direction.y == 1);

    //    this.AddConnection(section, atStart);
    //}

    //public void AddConnection(DungeonSection section, bool atStart = true)
    //{
    //    _connections[atStart ? 0 : 1] = section;
    //}

    public override string ToString()
    {
        return "Corridor[bounds:" + Bounds + "; direction:" + Direction + "; length: " + _length + "]";
    }

    public void RemoveLast()
    {
        Length--;
    }

    public void RemoveFirst()
    {
        Rect newBounds = new Rect(bounds);
        newBounds.position += _direction;
        Bounds = newBounds;
    }

    public Corridor[] RemoveAt(int index)
    {
        Corridor[] result = new Corridor[2];

        if (index > 0)
            result[0] = new Corridor(bounds.position, index, _direction);

        if (index < _length - 1)
            result[1] = new Corridor(this[index + 1], _length - index - 1, _direction);

        foreach (Room connection in connections)
        {
            if (result[0] && connection.ContainsCell(Pos - Direction))
                result[0].AddConnection(connection);
            else if (result[1] && connection.ContainsCell(LastCell + Direction))
                result[1].AddConnection(connection);

            connection.RemoveConnection(this);
        }
        return result;
    }
}