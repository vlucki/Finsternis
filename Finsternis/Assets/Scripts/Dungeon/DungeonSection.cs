using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonSection : IEnumerable<Vector2>
{
    protected Rect bounds;
    protected HashSet<DungeonSection> connections;

    public HashSet<DungeonSection> Connections { get { return connections; } }
    public Dictionary<Vector2, DungeonFeature> features;

    public Vector2 Size { get { return bounds.size; } }
    public float Width { get { return bounds.width; } }
    public float Height { get { return bounds.height; } }
    public float X { get { return bounds.x; } }
    public float Y { get { return bounds.y; } }

    public virtual Vector2 Position
    {
        get { return bounds.position;}
        set { bounds.position = value; }
    }
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
        features = new Dictionary<Vector2, DungeonFeature>();
    }

    public abstract bool ContainsCell(Vector2 cell);

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

    public T AddFeature<T>(Vector2 cell) where T : DungeonFeature
    {
        if(!features.ContainsKey(cell))
        {
            T t = ScriptableObject.CreateInstance<T>();
            features.Add(cell, t);
            return t;
        }
        return null;
    }

    public virtual DungeonFeature GetFeature(Vector2 cell)
    {
        DungeonFeature feature = null;
        features.TryGetValue(cell, out feature);
        return feature;
    }

    public virtual DungeonFeature GetFeature(float cellX, float cellY)
    {
        return GetFeature(new Vector2(cellX, cellY));
    }

    public abstract bool AddCell(Vector2 cell);

    public abstract void RemoveCell(Vector2 cell);
}

