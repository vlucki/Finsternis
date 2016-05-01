using UnityEngine;
public class Corridor : DungeonSection
{
    private Vector2 _direction;
    private int _length;
    

    public Vector2 Direction { get { return _direction; } set { _direction = value; } }
    public override Rect Bounds
    {
        get { return _bounds; }
        set
        {
            _bounds = value;
            _length = Mathf.RoundToInt(Mathf.Max(_bounds.size.x, _bounds.size.y));
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
                _bounds.size = new Vector2(value * _direction.x + _direction.y, value * _direction.y + _direction.x);
            }
            else
            {
                _length = 0;
                _bounds.size = Vector2.zero;
            }
        }
    }

    public Vector2 LastCell
    {
        get { return _length > 0 ? _bounds.max - Vector2.one : _bounds.position; }
    }

    public Vector2 this[int index]
    {
        get
        {
            if (index < 0 || index >= _length)
                throw new System.ArgumentOutOfRangeException("index", "Trying to access a cell with index " + index + " within " + ToString());
            return _bounds.position + _direction * index;
        }
    }

    public Corridor(Rect bounds, Vector2 direction)
    {
        _direction = direction;
        Bounds = bounds;
    }

    public Corridor(Vector2 position, int length, Vector2 direction)
    {
        _direction = direction;
        Bounds = new Rect(position, direction * length + new Vector2(direction.y, direction.x));
    }

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
        Rect newBounds = new Rect(_bounds);
        newBounds.position += _direction;
        Bounds = newBounds;
    }

    public Corridor[] RemoveAt(int index)
    {
        Corridor[] result = new Corridor[2];

        if (index > 0)
            result[0] = new Corridor(_bounds.position, index, _direction);

        if (index < _length - 1)
            result[1] = new Corridor(this[index + 1], _length - index - 1, _direction);

        return result;
    }
}