﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : DungeonSection
{
    private Vector2 _direction;
    private int _length;

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
        get { return _length > 0 ? this[Length-1] : Position; }
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
    }

    public Corridor(Vector2 position, int length, Vector2 direction) : 
        this(new Rect(position, direction * length + new Vector2(direction.y, direction.x)), direction)
    {
    }

    public void UpdateConnections()
    {
        foreach (DungeonSection connection in connections)
            connection.AddConnection(this);
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
        Rect newBounds = new Rect(bounds);
        newBounds.position += _direction;
        Bounds = newBounds;
    }

    public Corridor[] RemoveAt(int index)
    {
        if (index < 0 || index >= Length)
            throw new System.ArgumentOutOfRangeException("index", "Value should be in the range 0 " + Length);

        Corridor[] result = new Corridor[2];

        if (index > 0)
            result[0] = new Corridor(bounds.position, index, _direction);

        if (index < _length - 1)
            result[1] = new Corridor(this[index + 1], _length - index - 1, _direction);

        foreach (DungeonSection connection in connections)
        {
            if (result[0] && connection.ContainsCell(Position - Direction))
                result[0].AddConnection(connection);
            else if (result[1] && connection.ContainsCell(LastCell + Direction))
                result[1].AddConnection(connection);

            connection.RemoveConnection(this);
        }
        return result;
    }

    public override IEnumerator<Vector2> GetEnumerator()
    {
        for (int i = 0; i < _length; i++)
            yield return this[i];
    }

    public override bool ContainsCell(Vector2 cell)
    {
        return cell.x >= X && cell.x <= LastCell.x && cell.y >= Y && cell.y <= LastCell.y;
    }

    public override bool AddCell(Vector2 cell)
    {
        if (cell == this[0] - Direction)
        {
            Position -= Direction;
            Length++;
        }
        else if (cell == LastCell + Direction)
            Length++;
        else
            return false;

        return true;

    }

    public override void RemoveCell(Vector2 cell)
    {
        if(ContainsCell(cell))
        {
            while(LastCell != cell)
                Length--;
        }
    }
}