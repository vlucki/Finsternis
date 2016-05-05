using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonSection : IEnumerable<Vector2>
{
    protected Rect bounds;
    protected HashSet<DungeonSection> connections;

    public HashSet<DungeonSection> Connections { get { return connections; } }
    public Vector2 Pos { get { return bounds.position;} }
    public Vector2 Size { get { return bounds.size; } }
    public float Width { get { return bounds.width; } }
    public float Height { get { return bounds.height; } }
    public virtual Rect Bounds {
        get { return bounds; }
        set { }
    }

    public static implicit operator bool(DungeonSection section)
    {
        return section != null;
    } 


    public DungeonSection() : this(new Rect()) { }

    public DungeonSection(Rect bounds)
    {
        this.bounds = bounds;
        connections = new HashSet<DungeonSection>();
    }

    public void AddConnection(DungeonSection connection, bool updateNewConennection = false)
    {
        connections.Add(connection);
        if (updateNewConennection)
            connection.AddConnection(this);
    }

    public void RemoveConnection(DungeonSection connection)
    {
        connections.Remove(connection);
    }

    public abstract IEnumerator<Vector2> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

