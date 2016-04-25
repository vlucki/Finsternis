using UnityEngine;

public abstract class DungeonSection
{
    protected Rect _bounds;

    public Vector2 Pos { get { return _bounds.position;} }
    public Vector2 Size { get { return _bounds.size; } }
    public float Width { get { return _bounds.width; } }
    public float Height { get { return _bounds.height; } }
    public virtual Rect Bounds {
        get { return _bounds; }
        set { }
    }

    public DungeonSection() { _bounds = new Rect(); }
    public DungeonSection(Rect bounds) { _bounds = bounds; }
}

