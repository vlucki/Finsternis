using System.Collections.Generic;
using UnityEngine;

public class SimpleDungeon : Dungeon
{
    [SerializeField]
    public enum CellType
    {
        wall = 0,
        room = 10,
        corridor = 20,
        trappedFloor = 30
    }

    private List<Room> _rooms;
    private List<Corridor> _corridors;

    public bool allowDeadEnds = false;

    [SerializeField]
    private int[,] _dungeon;

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

    [Header("Minimum room dimensions")]
    [SerializeField]
    [Range(2, 1000)]
    private int _minimumRoomWidth = 3;

    [SerializeField]
    [Range(2, 1000)]
    private int _minimumRoomHeight = 3;

    [Header("Maximum room dimensions")]
    [SerializeField]
    [Range(2, 100)]
    private int _maximumRoomWidth = 7;

    [SerializeField]
    [Range(2, 100)]
    private int _maximumRoomHeight = 7;

    [Header("Corridor length")]
    [SerializeField]
    [Range(1, 100)]
    private int _minimumCorridorLength = 1;

    [SerializeField]
    [Range(1, 100)]
    private int _maximumCorridorLength = 5;

    private Room _lastRoom;

    public int Width { get { return _dungeon.GetLength(0); } }
    public int Height { get { return _dungeon.GetLength(1); } }

    public int[,] GetDungeon() { return _dungeon.Clone() as int[,]; }

    public int this[int x, int y]
    {
        get
        {
            try
            {
                return _dungeon[x, y];
            }
            catch (System.IndexOutOfRangeException ex)
            {
                throw new System.IndexOutOfRangeException("Attempting to access a cell outside of dungeon! [" + x + ";" + y + "]", ex);
            }
        }
        private set
        {
            try
            {
                _dungeon[x, y] = value;
            }
            catch (System.IndexOutOfRangeException ex)
            {
                throw new System.IndexOutOfRangeException("Attempting to access a cell outside of dungeon! [" + x + ";" + y + "]", ex);
            }
        }
    }

    public int this[float x, float y]
    {
        get { return this[(int)x, (int)y]; }
        private set { this[(int)x, (int)y] = value; }
    }

    public int this[Vector2 pos]
    {
        get { return this[(int)pos.x, (int)pos.y]; }
        private set { this[(int)pos.x, (int)pos.y] = value; }
    }

    public List<Corridor> Corridors { get { return _corridors; } }

    public override void Generate()
    {
        base.Generate();

        _dungeon = new int[_dungeonWidth, _dungeonHeight];
        _corridors = new List<Corridor>();
        _rooms = new List<Room>();

        _maximumRoomHeight  = Mathf.Clamp(_maximumRoomHeight, 0, Height);
        _maximumRoomWidth   = Mathf.Clamp(_maximumRoomWidth, 0, Width);
        _minimumRoomHeight  = Mathf.Clamp(_minimumRoomHeight, 0, _maximumRoomHeight);
        _minimumRoomWidth   = Mathf.Clamp(_minimumRoomWidth, 0, _maximumRoomWidth);

        _maximumCorridorLength = Mathf.Clamp(_maximumCorridorLength, 0, Mathf.Min(Height, Width));
        _minimumCorridorLength = Mathf.Clamp(_minimumCorridorLength, 0, _maximumCorridorLength);

        Queue<Corridor> hangingCorridors = null;
        Queue<Room> hangingRooms = new Queue<Room>();
        
        Vector2 maximumRoomSize = new Vector2(_maximumRoomWidth, _maximumRoomHeight);
        Vector2 minimumRoomSize = new Vector2(_minimumRoomWidth, _minimumRoomHeight);

        Room room;
        if (RoomFactory.CarveRoom(this, Vector2.zero, minimumRoomSize, maximumRoomSize, Vector2.zero, _maximumTries, out room))
        {
            MarkCells(room);
            hangingRooms.Enqueue(room);
            _rooms.Add(room);
            entrance = room.GetRandomCell();
        }
        else
            throw new System.InvalidOperationException("Could not create a single room within the dungeon. Maybe it is too small and the room too big?");

        int roomCount = 1;
        while (roomCount < _totalRooms  //keep going until the desired number of rooms was generated
                && (hangingRooms.Count > 0 || hangingCorridors.Count > 0)) //or until no more rooms or corridors can be generated
        {
            hangingCorridors = GenerateCorridors(hangingRooms);
            
            roomCount = GenerateRooms(hangingRooms, hangingCorridors, minimumRoomSize, maximumRoomSize, roomCount);            
        } 

        if(!allowDeadEnds) ConnectLeftoverCorridors(hangingCorridors);

        CleanUp();

        exit = _lastRoom.GetRandomCell();

        onGenerationEnd.Invoke();
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

    private void CleanUp()
    {
        List<Corridor> toAdd = new List<Corridor>();

        foreach(Corridor c in _corridors)
        {
            Corridor inUse = c;
            int originalLength = c.Length;
            bool haveToSplit = false;
            for(int i = 1; i <= originalLength; i++)
            {
                Vector2 offset = new Vector2(inUse.Direction.y, inUse.Direction.x);
                Vector2 cell = inUse[originalLength - i];
                if (    (!IsWithinDungeon(cell + offset) || this[cell + offset] == (int)CellType.wall) 
                     && (!IsWithinDungeon(cell - offset) || this[cell - offset] == (int)CellType.wall))
                {
                    haveToSplit = true;
                }
                else
                {
                    this[inUse[originalLength - i]] = (int)CellType.room;
                    if(!haveToSplit)
                        inUse.Length--;
                    else
                    {
                        Corridor[] parts = inUse.RemoveAt(originalLength - i);
                        toAdd.Remove(inUse);
                        inUse.Length = 0;

                        if (parts[1] != null)
                        {
                            toAdd.Add(parts[1]);
                        }

                        if(parts[0] != null)
                        {
                            toAdd.Add(parts[0]);
                            inUse = parts[0];
                            haveToSplit = false;
                        }
                        else
                        {
                            break;
                        }
                            
                    }
                }
            }
        }
        toAdd.ForEach(corridor => _corridors.Add(corridor));
        _corridors.RemoveAll(corridor => corridor.Length <= 0);

        //TODO: look into this and see if there's a problem here
        for (int i = _rooms.Count - 1; i > 0; i--)
        {
            Room roomA = _rooms[i];
            for (int j = i - 1; j >= 0; j--)
            {
                Room roomB = _rooms[j];
                if (roomA.Overlaps(roomB))
                {
                    roomA.Merge(roomB);
                    _rooms.RemoveAt(j);
                    i--;
                    j--;
                }
            }
        }
    }

    private void ConnectLeftoverCorridors(Queue<Corridor> hangingCorridors)
    {
        if(hangingCorridors.Count == 0)
            return;

        Queue<Corridor> corridorsStillHanging = new Queue<Corridor>(hangingCorridors.Count);

        while(hangingCorridors.Count > 0)
        {
            Corridor corridor = hangingCorridors.Dequeue();

            if (!ExtendCorridor(corridor)) //first of all, try to extend the corridor untils it intersects something
            {
                //if it fails, them remove every corridor that is not connected to another one
                if(!allowDeadEnds) corridorsStillHanging.Enqueue(corridor);
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
        
        while (corridor.Bounds.xMax < Width - _minimumRoomWidth * corridor.Direction.x
            && corridor.Bounds.yMax < Height - _minimumRoomHeight * corridor.Direction.y
            && this[corridor.LastCell + corridor.Direction] == (int)CellType.wall)
        {
            corridor.Length++;
        }

        if (oldLength != corridor.Length
            && this[corridor.LastCell] != (int)CellType.wall)
        {
            MarkCells(corridor.Bounds.position, corridor.Bounds.size, CellType.corridor);
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

        Vector2 offset = new Vector2(corridor.Direction.y, corridor.Direction.x);

        while(corridor.Length > 0 && !intersectionFound)
        {
            //look around the last cell of the corridor
            for (int i = -1; i < 2; i += 2)
            {
                Vector2 adjacentCell = corridor.LastCell + offset * i;
                if (IsWithinDungeon(adjacentCell))
                {
                    //if an adjacent corridor was found, stop searching
                    if (this[adjacentCell] == (int)CellType.corridor)
                    {
                        intersectionFound = true;
                        break;
                    }
                }
            }
            if (!intersectionFound)
            {
                //Remove "excess cells" from the corridor
                this[corridor.LastCell] = (int)CellType.wall;
                corridor.Length--;
            }
        }

        if(corridor.Length != originalLength)
        {
            if(corridor.Length == 0)
                _corridors.Remove(corridor);
        }
    }

    /// <summary>
    /// Generate a room for every hanging corridor (corridors with no room attatched to it's end).
    /// </summary>
    /// <param name="hangingRooms">Rooms generated.</param>
    /// <param name="hangingCorridors">Corridors that need rooms attatched to their end.</param>
    /// <param name="minimumRoomSize">Minimum width and height for the rooms.</param>
    /// <param name="maximumRoomSize">Maximum width and height for the rooms.</param>
    /// <returns>How many rooms were created.</returns>
    public int GenerateRooms(Queue<Room> hangingRooms, Queue<Corridor> hangingCorridors, Vector2 minimumRoomSize, Vector2 maximumRoomSize, int roomCount)
    {
        //until there are no hanging corridors (that is, corridors with rooms only at their start) 
        while (hangingCorridors.Count > 0 && roomCount < _totalRooms)
        {
            //make a room at the end of a corridor and add it to the queue of rooms without corridors
            Room room;
            Corridor corridor = hangingCorridors.Dequeue();

            if (RoomFactory.CarveRoom(this, corridor, minimumRoomSize, maximumRoomSize, _maximumTries, out room))
            {
                hangingRooms.Enqueue(room);
                MarkCells(room);
                _rooms.Add(room);
                _lastRoom = room;

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
            if (CorridorFactory.CarveCorridor(this, room, Vector2.right, new Vector2(_minimumCorridorLength, _maximumCorridorLength), new Vector2(_minimumRoomWidth, _minimumRoomHeight), out corridor))
            {
                hangingCorridors.Enqueue(corridor);
                _corridors.Add(corridor);
                MarkCells(corridor.Pos, corridor.Size, CellType.corridor, false);
                if (Random.value > 0.8f)
                    this[corridor.Pos + corridor.Direction * Random.Range(0, corridor.Length)] = (int)CellType.trappedFloor;

            }

            if (CorridorFactory.CarveCorridor(this, room, Vector2.up, new Vector2(_minimumCorridorLength, _maximumCorridorLength), new Vector2(_minimumRoomWidth, _minimumRoomHeight), out corridor))
            {
                hangingCorridors.Enqueue(corridor);
                _corridors.Add(corridor);
                MarkCells(corridor.Pos, corridor.Size, CellType.corridor, false);
                if (Random.value > 0.8f)
                    this[corridor.Pos + corridor.Direction * Random.Range(0, corridor.Length)] = (int)CellType.trappedFloor;

            }
        }

        return hangingCorridors;
    }

    /// <summary>
    /// Searches a given area for a given cell type;
    /// </summary>
    /// <param name="pos">The area starting point.</param>
    /// <param name="size">The width and height of the search area.</param>
    /// <param name="type">The type of cell that is being searched.</param>
    /// <returns></returns>
    public bool SearchInArea(Vector2 pos, Vector2 size, CellType type)
    {
        for(int row = (int)pos.y; row < Height && row < pos.y + size.y; row++)
        {
            for (int col = (int)pos.x; col < Width && col < pos.x + size.x; col++)
            {
                if (this[col, row] == (int)type)
                    return true;
            }
        }

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

                if(IsWithinDungeon(x, y)
                    && this[x, y] == (int)CellType.wall)
                {
                    return true;
                }

            }
        }

        return false;
    }

    /// <summary>
    /// Marks every cell within the given area.
    /// </summary>
    /// <param name="start">Upper left corner of the area to be marked.</param>
    /// <param name="size">Width and height of the area to be marked.</param>
    /// <param name="type">Type to use when marking the cells.</param>
    /// <param name="ignoreIntersection">If it should ignore cells that are not empty.</param>
    /// <returns>The point where there was an intersection or where the marking of cells stopped.</returns>
    private Vector2 MarkCells(Vector2 start, Vector2 size, CellType type = CellType.room, bool ignoreIntersection = true)
    {
        for (int row = (int)start.y; row < start.y + size.y && row < Height; row++)
        {
            for (int col = (int)start.x; col < start.x + size.x && col < Width; col++)
            {
                if (this[col, row] != (int)CellType.wall && !ignoreIntersection)
                    return new Vector2(col, row);

                this[col, row] = (int)type;                
            }
        }

        return start+size;
    }

    private void MarkCells(Room room)
    {
        room.Cells.ForEach(cell => this[cell] = (int)CellType.room);
    }

    private void MarkCells(Corridor corridor)
    {
        MarkCells(corridor.Bounds.position, corridor.Bounds.size, CellType.corridor);
    }
}
