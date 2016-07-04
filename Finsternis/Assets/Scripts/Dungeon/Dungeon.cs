using System;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    [SerializeField]
    private int _seed;

    [SerializeField]
    private bool customSeed = true;

    [SerializeField]
    protected Vector2 entrance;

    [SerializeField]
    protected Vector2 exit;

    protected MTRandom random;

    protected int availableCardPoints;
    
    protected List<Room> _rooms;
    protected List<Corridor> _corridors;
    protected DungeonSection[,] _dungeon;
    protected int killsUntilNext;


    public int KillsUntilNext { get { return killsUntilNext; } set { killsUntilNext = value; } }

    public MTRandom Random { get { return random; } }

    public int AvailableCardPoints { get { return availableCardPoints; } }

    public int Seed
    {
        get { return _seed; }
        set
        {
            if (customSeed)
            {
                random = new MTRandom(value);
                _seed = value;
            }
        }
    }

    public Vector2 Entrance { get { return entrance; } set { entrance = value; } }
    public Vector2 Exit { get { return exit; } set { exit = value; } }
    public int Width { get { return _dungeon.GetLength(0); } }
    public int Height { get { return _dungeon.GetLength(1); } }

    public Vector2 Size { get { return new Vector2(Width, Height); } }

    public int[,] GetDungeon() { return _dungeon.Clone() as int[,]; }

    public DungeonSection this[int x, int y]
    {
        get
        {
            try
            {
                return _dungeon[x, y];
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new IndexOutOfRangeException("Attempting to access a cell outside of dungeon! [" + x + ";" + y + "]", ex);
            }
        }
        set
        {
            try
            {
                _dungeon[x, y] = value;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new IndexOutOfRangeException("Attempting to access a cell outside of dungeon! [" + x + ";" + y + "]", ex);
            }
        }
    }

    public DungeonSection this[float x, float y]
    {
        get { return this[(int)x, (int)y]; }
        set { this[(int)x, (int)y] = value; }
    }

    public DungeonSection this[Vector2 pos]
    {
        get { return this[(int)pos.x, (int)pos.y]; }
        set { this[(int)pos.x, (int)pos.y] = value; }
    }

    public List<Corridor> Corridors { get { return _corridors; } }
    public List<Room> Rooms { get { return _rooms; } }

    public Room GetRandomRoom()
    {
        return _rooms[random.Range(0, _rooms.Count, false)];
    }

    public void EnemyKilled()
    {
        killsUntilNext--;
    }

    public bool IsWithinDungeon(Vector2 cell)
    {
        return IsWithinDungeon(cell.x, cell.y);
    }

    public bool IsWithinDungeon(float x, float y)
    {
        return x >= 0
            && x < Width
            && y >= 0
            && y < Height;
    }

    /// <summary>
    /// Searches a given area for a given cell type;
    /// </summary>
    /// <param name="pos">The area starting point.</param>
    /// <param name="size">The width and height of the search area.</param>
    /// <param name="types">The types of cell that are being searched.</param>
    /// <returns></returns>
    public bool SearchInArea(Vector2 pos, Vector2 size, params Type[] types)
    {
        for (int row = (int)pos.y; row < Height && row < pos.y + size.y; row++)
        {
            for (int col = (int)pos.x; col < Width && col < pos.x + size.x; col++)
            {
                Vector2 cell = new Vector2(col, row);
                if (IsWithinDungeon(cell) && IsOfAnyType(cell, types))
                    return true;
            }
        }

        return false;
    }

    public int SearchAround(Vector2 coord, int stopAt, bool checkDiagonals, params Type[] types)
    {
        if (types == null || types.Length < 1)
            throw new InvalidOperationException("Must provide at least one type to be searched for.");

        int cellsFound = 0;

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == j)
                    continue;
                if (!checkDiagonals && Mathf.Abs(i) == Mathf.Abs(j))
                    continue;
                Vector2 cell = coord - new Vector2(i, j);
                if (IsWithinDungeon(cell) && IsOfAnyType(cell, types))
                    cellsFound++;
                if (stopAt > 0 && cellsFound == stopAt)
                    return cellsFound;
            }
        }

        return cellsFound;
    }

    /// <summary>
    /// Checks if a given rectangle overlaps a corridor
    /// </summary>
    /// <param name="pos">Upper left corner of the rectangle.</param>
    /// <param name="size">Dimenstions of the rectangle.</param>
    /// <returns>True if it does overlap.</returns>
    internal bool OverlapsCorridor(Vector2 pos, Vector2 size)
    {
        Rect r = new Rect(pos, size);

        foreach (Corridor c in _corridors)
        {
            if (c.Bounds.Overlaps(r))
                return true;
        }
        return false;
    }

    public bool IsOfAnyType(Vector2 cell, params Type[] types)
    {
        foreach (Type type in types)
        {
            if (IsOfType(cell, type))
                return true;
        }
        return false;
    }
    public bool IsOfType(float cellX, float cellY, Type type)
    {
        if (this[cellX, cellY])
        {
            if (type.Equals(this[cellX, cellY].GetType()))
                return true;
        }
        else if (type == null) //Wall
            return true;

        return false;
    }

    public bool IsOfType(Vector2 cell, Type type)
    {
        if (this[cell])
        {
            if (type.Equals(this[cell].GetType()))
                return true;
        }
        else if (type == null) //Wall
            return true;

        return false;
    }

    /// <summary>
    /// Searches for unmarked cells around a given point in the dungeon.
    /// </summary>
    /// <param name="cell">The center point for the search.</param>
    /// <param name="ignoreDiagonal">If the diagonals should not be checked.</param>
    /// <returns>True if there is at least one unmarked cell around the given point.</returns>
    private bool IsEdgeCell(Vector2 cell, bool ignoreDiagonal = true)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if ((ignoreDiagonal && Mathf.Abs(i) == Mathf.Abs(j)) || (i == 0 && i == j)) //ignore diagonals (if requested) and the cell itself (when both i and j are 0)
                    continue;

                int y = (int)(cell.y + i);
                int x = (int)(cell.x + j);

                if (IsWithinDungeon(x, y)
                    && this[x, y] == null)
                {
                    return true;
                }

            }
        }

        return false;
    }

    public void MarkCells(DungeonSection section)
    {
        foreach (Vector2 cell in section)
        {
            this[cell] = section;
        }
    }

    //public virtual void Awake()
    //{
    //    if (customSeed)
    //    {
    //        random = new MTRandom(this._seed);
    //    }
    //}

    public void Init(int width, int height)
    {
        if (customSeed)
        {
            random = new MTRandom(this._seed);
        }
        _dungeon = new DungeonSection[width, height];
        _corridors = new List<Corridor>();
        _rooms = new List<Room>();
    }
}

