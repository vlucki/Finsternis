using System;
using System.Collections.Generic;
using UnityEngine;


//TODO: debug seed 221, where corridor does not connect to other
public class SimpleDungeon : Dungeon
{
    private List<Room> _rooms;
    private List<Corridor> _corridors;

    public bool allowDeadEnds = false;

    private DungeonSection[,] _dungeon;

    [Header("Dungeon dimensions")]
    [SerializeField]
    [Range(10, 1000)]
    private int _dungeonWidth = 20;

    [SerializeField]
    [Range(10, 1000)]
    private int _dungeonHeight = 20;

    [SerializeField]
    [Range(2, 1000)] //at least one starting point and one exit
    private int _totalRooms = 5;

    [Tooltip("How many times should it try to carve a room before stoping")]
    [SerializeField]
    [Range(1, 100)]
    private int _maximumTries = 2;

    [Header("Maximum room size")]
    [SerializeField]
    [Range(2, 1000)]
    private int _maximumRoomWidth;

    [SerializeField]
    [Range(2, 1000)]
    private int _maximumRoomHeight;

    [Header("Minimum brush size")]
    [SerializeField]
    [Range(2, 1000)]
    private int _minimumBrushWidth = 3;

    [SerializeField]
    [Range(2, 1000)]
    private int _minimumBrushHeight = 3;

    [Header("Maximum brish size")]
    [SerializeField]
    [Range(2, 100)]
    private int _maximumBrushWidth = 7;

    [SerializeField]
    [Range(2, 100)]
    private int _maximumBrushHeight = 7;

    [Header("Corridor length")]
    [SerializeField]
    [Range(1, 100)]
    private int _minimumCorridorLength = 1;

    [SerializeField]
    [Range(1, 100)]
    private int _maximumCorridorLength = 5;

    private Room _lastRoom;

    public int killsUntilNext = 0;

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
        private set
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
        private set { this[(int)x, (int)y] = value; }
    }

    public DungeonSection this[Vector2 pos]
    {
        get { return this[(int)pos.x, (int)pos.y]; }
        private set { this[(int)pos.x, (int)pos.y] = value; }
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

    private void Init()
    {
        _dungeon = new DungeonSection[_dungeonWidth, _dungeonHeight];
        _corridors = new List<Corridor>();
        _rooms = new List<Room>();

        _maximumBrushHeight = Mathf.Clamp(_maximumBrushHeight, 0, Height);
        _maximumBrushWidth = Mathf.Clamp(_maximumBrushWidth, 0, Width);
        _minimumBrushHeight = Mathf.Clamp(_minimumBrushHeight, 0, _maximumBrushHeight);
        _minimumBrushWidth = Mathf.Clamp(_minimumBrushWidth, 0, _maximumBrushWidth);
        _maximumCorridorLength = Mathf.Clamp(_maximumCorridorLength, 0, Mathf.Min(Height, Width));
        _minimumCorridorLength = Mathf.Clamp(_minimumCorridorLength, 0, _maximumCorridorLength);
    }

    public override void Generate()
    {
#if UNITY_EDITOR
        base.Awake();
#endif

        base.Generate();

        Init();

        Queue<Corridor> hangingCorridors = null;
        Queue<Room> hangingRooms = new Queue<Room>();

        Vector4 brushVariation = new Vector4(_minimumBrushWidth, _minimumBrushHeight, _maximumBrushWidth, _maximumBrushHeight);
        Vector2 maxRoomSize = new Vector2(_minimumBrushWidth, _minimumBrushHeight);

        Room room;
        if (RoomFactory.CarveRoom(this, null, brushVariation, maxRoomSize, _maximumTries, out room))
        {
            MarkCells(room);
            hangingRooms.Enqueue(room);
            _rooms.Add(room);
            entrance = room.GetRandomCell();
        }
        else
            throw new InvalidOperationException("Could not create a single room within the dungeon. Maybe it is too small and the room too big?");

        int roomCount = 1;
        while (roomCount < _totalRooms  //keep going until the desired number of rooms was generated
                && (hangingRooms.Count > 0 || hangingCorridors.Count > 0)) //or until no more rooms or corridors can be generated
        {
            hangingCorridors = GenerateCorridors(hangingRooms);

            roomCount = GenerateRooms(hangingRooms, hangingCorridors, brushVariation, maxRoomSize, roomCount);
        }

        if (!allowDeadEnds) ConnectLeftoverCorridors(hangingCorridors);

        CleanUp();

        AddFeatures();

        exit = _lastRoom.GetRandomCell();

        Seed = random.Range(0, 0xFFF);

        onGenerationEnd.Invoke();
    }

    private void AddFeatures()
    {
        foreach(Corridor corridor in _corridors)
        {
            if (corridor.Length > 2 && Random.value() <= 0.3f)
            {
                int pos = Random.Range(1, corridor.Length - 2);
                corridor.AddFeature<Trap>(corridor[pos]).Id = Random.Range(0, 1);
            }
        }
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
    /// Removes corridors that don't really look like corridors (eg. have walls only on one side) 
    /// and merge rooms that overlap or don't have walls between one or more cells
    /// </summary>
    private void CleanUp(int iteration = 0, int limit = 10)
    {
        FixCorridors();
        MergeRooms();
    }

    private void FixCorridors()
    {
        for (int i = _corridors.Count - 1; i >= 0; i--)
        {
            Corridor c = _corridors[i];
            if (SplitCorridor(c))
            {
                i = _corridors.Count;
                continue;
            }
            if (!ExtendCorridor(c))
                TrimCorridor(c);
        }
    }

    private bool SplitCorridor(Corridor c)
    {
        Corridor[] halves = null;
        Vector2 offset = new Vector2(c.Direction.y, c.Direction.x);
        bool hasToSplit = false;
        int index = c.Length - 1;
        while (index >= 0)
        {
            Vector2 cell = c[index];
            Vector2 offsetCellA = cell + offset;
            Vector2 offsetCellB = cell - offset;
            if (IsWithinDungeon(offsetCellA))
            {
                if (this[offsetCellA] && !(this[offsetCellA] is Corridor))
                {
                    this[cell] = this[offsetCellA];
                    this[offsetCellA].AddCell(cell);
                    if (hasToSplit)
                    {
                        halves = c.RemoveAt(index);
                        break;
                    }
                    else
                    {
                        c.Length--;
                    }
                }
            }
            if (IsWithinDungeon(offsetCellB))
            {
                if (this[offsetCellB] && !(this[offsetCellB] is Corridor))
                {
                    this[cell] = this[offsetCellB];
                    this[offsetCellB].AddCell(cell);
                    if (hasToSplit)
                    {
                        halves = c.RemoveAt(index);
                        break;
                    }
                    else
                    {
                        c.Length--;
                    }
                }
            }
            index--;
            if (index >= c.Length)
                index = c.Length - 1;
            hasToSplit = true;
        }
        if(halves != null)
        {
            if (halves[0])
                _corridors.Add(halves[0]);
            if (halves[1])
                _corridors.Add(halves[1]);
            _corridors.Remove(c);
        }
        if (c.Length == 0)
            _corridors.Remove(c);
        return halves != null;
    }

    void MergeRooms()
    {
        for (int i = _rooms.Count - 1; i > 0; i--)
        {
            Room roomA = _rooms[i];
            for (int j = i - 1; j >= 0; j--)
            {
                Room roomB = _rooms[j];

                if (roomA.IsTouching(roomB))
                {
                    roomA.Merge(roomB);
                    roomB.Disconnect();
                    _rooms.RemoveAt(j);
                    i = _rooms.Count;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Tries to connect every corridor that still isn't connected to something.
    /// </summary>
    /// <param name="hangingCorridors">The corridors that still are not attached to another room section (ie. have a dead end)</param>
    private void ConnectLeftoverCorridors(Queue<Corridor> hangingCorridors)
    {
        if (hangingCorridors.Count == 0)
            return;

        Queue<Corridor> corridorsStillHanging = new Queue<Corridor>(hangingCorridors.Count);

        while (hangingCorridors.Count > 0)
        {
            Corridor corridor = hangingCorridors.Dequeue();
            if (!ExtendCorridor(corridor)) //first of all, try to extend the corridor untils it intersects something
            {
                //if it fails, them remove every corridor that is not connected to another one
                if (!allowDeadEnds) corridorsStillHanging.Enqueue(corridor);
            }
        }

        //Must only trim corridors after trying to extend them all!
        while (corridorsStillHanging.Count > 0)
        {
            TrimCorridor(corridorsStillHanging.Dequeue());
        }
    }

    /// <summary>
    /// Extends a corridor until it reaches a room or another corridor.
    /// </summary>
    /// <param name="corridor">Corridor to be extended.</param>
    /// <returns>True if the corridor was succesfully extended.</returns>
    private bool ExtendCorridor(Corridor corridor)
    {
        int oldLength = corridor.Length;

        while (corridor.Bounds.xMax < Width - corridor.Direction.x
            && corridor.Bounds.yMax < Height - corridor.Direction.y
            && this[corridor.LastCell + corridor.Direction] == null)
        {
            corridor.Length++;
        }

        if (oldLength != corridor.Length
            && this[corridor.LastCell + corridor.Direction] != null)
        {
            MarkCells(corridor);
            return true;
        }
        else
        {
            corridor.Length = oldLength;
            return false;
        }
    }

    /// <summary>
    /// Reduces the length of a corridor in order for it not to be a dead end.
    /// </summary>
    /// <param name="corridor">Corridor to be reduced.</param>
    private void TrimCorridor(Corridor corridor)
    {
        int originalLength = corridor.Length;

        bool intersectionFound = false;

        while (corridor.Length > 0 && !intersectionFound)
        {
            //look around the last cell of the corridor
            intersectionFound = (SearchAround(corridor.LastCell, 2, false, typeof(Corridor), typeof(Room)) >= 2);

            if (!intersectionFound)
            {
                //Remove "excess cells" from the corridor
                this[corridor.LastCell] = null;
                corridor.Length--;
            }
        }

        if (corridor.Length != originalLength)
        {
            if (corridor.Length < _minimumCorridorLength)
                _corridors.Remove(corridor);
        }
    }

    private int SearchAround(Vector2 coord, int stopAt, bool checkDiagonals, params Type[] types)
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
    /// Generate a room for every hanging corridor (corridors with no room attatched to it's end).
    /// </summary>
    /// <param name="hangingRooms">Rooms generated.</param>
    /// <param name="hangingCorridors">Corridors that need rooms attatched to their end.</param>
    /// <param name="minimumRoomSize">Minimum width and height for the rooms.</param>
    /// <param name="maximumRoomSize">Maximum width and height for the rooms.</param>
    /// <returns>How many rooms were created.</returns>
    public int GenerateRooms(Queue<Room> hangingRooms, Queue<Corridor> hangingCorridors, Vector4 brushVariation, Vector2 maxRoomSize, int roomCount)
    {
        //until there are no hanging corridors (that is, corridors with rooms only at their start) 
        while (hangingCorridors.Count > 0 && roomCount < _totalRooms)
        {
            //make a room at the end of a corridor and add it to the queue of rooms without corridors
            Room room;
            Corridor corridor = hangingCorridors.Dequeue();


            if (RoomFactory.CarveRoom(this, corridor, brushVariation, maxRoomSize, _maximumTries, out room))
            {
                hangingRooms.Enqueue(room);
                MarkCells(room);
                _rooms.Add(room);
                _lastRoom = room;
                corridor.AddConnection(room);
                roomCount++;
            }
            else
            {
                ExtendCorridor(corridor);
            }
        }
        return roomCount;
    }


    /// <summary>
    /// Generate corridors for the rooms that still don't have them.
    /// </summary>
    /// <param name="hangingRooms">Rooms without corridors going out of them.</param>
    /// <returns>Every corridor generated.</returns>
    private Queue<Corridor> GenerateCorridors(Queue<Room> hangingRooms)
    {
        Queue<Corridor> hangingCorridors = new Queue<Corridor>(hangingRooms.Count); //assume at least one corridor per room at first

        //until every room has a corridor going out of it, make corridors
        while (hangingRooms.Count > 0)
        {
            Room room = hangingRooms.Dequeue();

            Corridor corridor;
            for (int i = 0; i < 2; i++)
            {
                if (CorridorFactory.CarveCorridor(this, room, (i == 0 ? Vector2.right : Vector2.up), new Vector2(_minimumCorridorLength, _maximumCorridorLength), new Vector2(_minimumBrushWidth, _minimumBrushHeight), out corridor))
                {
                    hangingCorridors.Enqueue(corridor);
                    _corridors.Add(corridor);
                    corridor.AddConnection(room);
                    MarkCells(corridor);
                }
            }
        }

        return hangingCorridors;
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

    private void MarkCells(DungeonSection section)
    {
        foreach (Vector2 cell in section)
        {
            this[cell] = section;
        }
    }
}
