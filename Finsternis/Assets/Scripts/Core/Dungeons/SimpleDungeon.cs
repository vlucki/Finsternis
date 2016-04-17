using System.Collections.Generic;
using UnityEngine;
using Room = RoomFactory.Room;

public class SimpleDungeon : Dungeon
{

    public class Corridor
    {
        public Vector2 Direction { get; set; }
        public Rect Bounds { get; set; }

        public Corridor(Rect bounds, Vector2 direction)
        {
            Bounds = bounds;
            Direction = direction;
        }

        public override string ToString()
        {
            return "Corridor[bounds:" + Bounds + "; directions:" + Direction + "]";
        }
    }


    System.Diagnostics.Stopwatch profilingTimer;

    public enum DebugLevel
    {
        NONE = 0,
        MINIMAL = 10,
        IN_DEPTH = 20,
        COMPLETE = 100
    }

    public DebugLevel debugLevel = DebugLevel.NONE;
    public bool profilingOn = true;

    [SerializeField]
    public enum CellType
    {
        wall = 0,
        room = 10,
        corridor = 20
    }

    [SerializeField]
    private CellType[,] _dungeon;

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

    [SerializeField]
    private Vector2 _entrance;

    [SerializeField]
    private Vector2 _exit;

    private Room _lastRoom;

    public Vector2 Entrance { get { return _entrance; } }
    public Vector2 Exit { get { return _exit; } }

    public int Width { get { return _dungeonWidth; } }
    public int Height { get { return _dungeonHeight; } }

    public CellType[,] GetDungeon() { return _dungeon.Clone() as CellType[,]; }

    public CellType this[int y, int x]
    {
        get { return _dungeon[y, x]; }
    }

    //TODO: debug only code - REMOVE
    private void DisplayDungeon()
    {

        string dungeon = "";
        for (int row = -1; row < _dungeonHeight; row++)
        {
            if (row >= 0)
                dungeon += (row > 9 ? "" : "0") + row;
            else
                dungeon += "---";

            for (int col = 0; col < _dungeonWidth; col++)
            {
                dungeon += "|";

                if (row < 0)
                {
                    dungeon += (col % 2 != 0 ? " " : "") + (col > 9 ? "" : "0") + col;
                }
                else
                {
                    switch (_dungeon[row, col])
                    {
                        case CellType.wall:
                            dungeon += " <color=red>X</color> ";
                            break;
                        case CellType.room:
                            dungeon += " <color=cyan>R</color> ";
                            break;
                        case CellType.corridor:
                            dungeon += "    ";
                            break;
                    }
                }
            }
            dungeon += "|\n";
        }
        dungeon += "";
        Debug.Log(dungeon);

    }

    public override void Generate()
    {
        base.Generate();

        if (profilingOn) profilingTimer = new System.Diagnostics.Stopwatch();
        if (debugLevel >= DebugLevel.MINIMAL)
        {
            Debug.Log("<b>GENERATING DUNGEON</b>");
            Debug.Log("Current seed = " + Random.seed);
        }

        _dungeon = new CellType[_dungeonHeight, _dungeonWidth];
        Queue<Corridor> hangingCorridors = null;
        Queue<Room> hangingRooms = new Queue<Room>();
        
        Vector2 maximumRoomSize = new Vector2(_maximumRoomWidth, _maximumRoomHeight);
        Vector2 minimumRoomSize = new Vector2(_minimumRoomWidth, _minimumRoomHeight);
        
        long lastElapsedTime = 0;

        int roomCount = 0;
        do
        {
            if (roomCount == 0) //carve the very first room
            {
                if (debugLevel >= DebugLevel.MINIMAL) Debug.Log("<b>Creating first room</b>");
                if (profilingOn) profilingTimer.Start();

                Room room;
                if (CarveRoom(Vector2.zero, minimumRoomSize, maximumRoomSize, Vector2.zero, out room))
                {
                    hangingRooms.Enqueue(room);
                    _entrance = room.GetRandomCell();
                }
                else
                    throw new System.InvalidOperationException("Could not create a single room within the dungeon. Maybe it is too small and the room too big?");

                if (profilingOn)
                {
                    profilingTimer.Stop();
                    Debug.Log("<b>Finished creating first room <color=red>(" + (profilingTimer.ElapsedMilliseconds - lastElapsedTime) + "ms)</color></b>");
                    lastElapsedTime = profilingTimer.ElapsedMilliseconds;
                }

                roomCount++;
            }
            else
            {
                if (profilingOn) profilingTimer.Start();

                hangingCorridors = GenerateCorridors(hangingRooms);

                if (profilingOn)
                {
                    profilingTimer.Stop();
                    Debug.Log("<b>Finished creating corridors <color=red>(" + (profilingTimer.ElapsedMilliseconds - lastElapsedTime) + "ms)<color></b>");
                    lastElapsedTime = profilingTimer.ElapsedMilliseconds;
                }

                if (profilingOn) profilingTimer.Start();

                roomCount = GenerateRooms(hangingRooms, hangingCorridors, minimumRoomSize, maximumRoomSize, roomCount);

                if (profilingOn)
                {
                    profilingTimer.Stop();
                    Debug.Log("<b>Finished creating rooms <color=red>(" + (profilingTimer.ElapsedMilliseconds - lastElapsedTime) + "ms)</color></b>");
                    lastElapsedTime = profilingTimer.ElapsedMilliseconds;
                }
            }
            
        } while (roomCount < _totalRooms  //keep going until the desired number of rooms was generated
                && (hangingRooms.Count > 0 || hangingCorridors.Count > 0)); //or until no more rooms or corridors can be generated

        if(profilingOn) profilingTimer.Start();

        ConnectLeftoverCorridors(hangingCorridors);

        if (profilingOn)
        {
            profilingTimer.Stop();
            Debug.Log("<b>Finished making connections <color=red>(" + (profilingTimer.ElapsedMilliseconds - lastElapsedTime) + "ms)</color></b>");
            Debug.Log("<b>DUNGEON GENERATED <color=red>(" + profilingTimer.ElapsedMilliseconds + "ms)</color></b>");
        }

        _exit = _lastRoom.GetRandomCell();

        if (debugLevel >= DebugLevel.MINIMAL) Debug.Log("Dungeon exit = " + _exit);

        onGenerationEnd.Invoke();
    }

    private void ConnectLeftoverCorridors(Queue<Corridor> hangingCorridors)
    {
        if(hangingCorridors.Count == 0)
        {
            if (debugLevel >= DebugLevel.MINIMAL) Debug.Log("No corridors leftover to be trimmed.");
            return;
        }

        if (debugLevel >= DebugLevel.MINIMAL) Debug.Log("<b>Connecting leftover corridors</b>");

        Queue<Corridor> corridorsStillHanging = new Queue<Corridor>(hangingCorridors.Count);

        while(hangingCorridors.Count > 0)
        {
            Corridor corridor = hangingCorridors.Dequeue();

            if (!ExtendCorridor(corridor)) //first of all, try to extend the corridor untils it intersects something
            {
                //if it fails, them remove every corridor that is not connected to another one
                corridorsStillHanging.Enqueue(corridor);

            }
        }

        while (corridorsStillHanging.Count > 0)
        {
            TrimCorridor(corridorsStillHanging.Dequeue());
        }
    }

    /// <summary>
    /// Reduces the length of a corridor in order for it not to be a dead end.
    /// </summary>
    /// <param name="corridor">Corridor to be reduced.</param>
    private void TrimCorridor(Corridor corridor)
    {
        if (debugLevel >= DebugLevel.IN_DEPTH) Debug.Log("<b>Trimming corridor: " + corridor + "</b>");

        Rect bounds = corridor.Bounds;
        Vector2 trimEnd = bounds.max;
        bool intersectionFound = false;
        //start at the end of the corridor and go towards its beginning
        for (int row = (int)(bounds.yMax - 1); !intersectionFound && row >= bounds.y; row--)
        {
            for (int col = (int)(bounds.xMax - 1); !intersectionFound && col >= bounds.x; col--)
            {
                if (corridor.Direction.x == 0) //corridor is vertical
                {
                    if ((col > 0 && _dungeon[row, col - 1] == CellType.corridor) //there is a corridor to the left
                        || (col + 1 < _dungeonWidth && _dungeon[row, col + 1] == CellType.corridor)) //there is a corridor do the right
                    {
                        intersectionFound = true;
                    }
                }
                else if ((row > 0 && _dungeon[row - 1, col] == CellType.corridor) //there is a corridor above
                        || (row + 1 < _dungeonHeight && _dungeon[row + 1, col] == CellType.corridor)) //there if a corridor below
                {
                    intersectionFound = true;
                }
                trimEnd.x = col;
            }
            trimEnd.y = row;
        }

        if(trimEnd != bounds.max)
        {
            if (debugLevel >= DebugLevel.IN_DEPTH) Debug.Log("Trimming corridor " + trimEnd + " - " + bounds.max);
            MarkCells(trimEnd, bounds.max - trimEnd, CellType.wall);
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

        if (debugLevel >= DebugLevel.MINIMAL) Debug.Log("<b>Creating rooms</b>");

        //until there are no hanging corridors (that is, corridors with rooms only at their start) 
        while (hangingCorridors.Count > 0 && roomCount < _totalRooms)
        {
            //make a room at the end of a corridor and add it to the queue of rooms without corridors
            Room room;
            Corridor corridor = hangingCorridors.Dequeue();

            if (CarveRoom(corridor.Bounds.max, minimumRoomSize, maximumRoomSize, corridor.Direction, out room))
            {
                hangingRooms.Enqueue(room);

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
    /// Extends a corridor until it reaches a room or another corridor.
    /// </summary>
    /// <param name="corridor">Corridor to be extended.</param>
    /// <returns>True if the corridor was succesfully extended.</returns>
    private bool ExtendCorridor(Corridor corridor)
    {
        if (debugLevel >= DebugLevel.IN_DEPTH) Debug.Log("<b>Extending corridor: </b>" + corridor);

        Vector2 newCorridorEnd = corridor.Bounds.max;

        while(newCorridorEnd.x < _dungeonWidth 
            && newCorridorEnd.y < _dungeonHeight
            && _dungeon[(int)(newCorridorEnd.y - corridor.Direction.x), (int)(newCorridorEnd.x - corridor.Direction.y)] == CellType.wall)
        {
            newCorridorEnd += corridor.Direction;
        }

        if (newCorridorEnd != corridor.Bounds.max
            && newCorridorEnd.x < _dungeonWidth
            && newCorridorEnd.y < _dungeonHeight
            && _dungeon[(int)(newCorridorEnd.y - corridor.Direction.x), (int)(newCorridorEnd.x - corridor.Direction.y)] != CellType.wall)
        {
            corridor.Bounds = new Rect(corridor.Bounds.x, corridor.Bounds.y, newCorridorEnd.x - corridor.Bounds.xMin, newCorridorEnd.y - corridor.Bounds.yMin);

            if (debugLevel >= DebugLevel.IN_DEPTH) Debug.Log("Corridor successfully extended - " + corridor);

            MarkCells(corridor.Bounds.position, corridor.Bounds.size, CellType.corridor);
            return true;
        }
        else
        {
            if (debugLevel >= DebugLevel.IN_DEPTH) Debug.Log("Impossible to extend the corridor from " + corridor.Bounds.max + " - stopped at " + newCorridorEnd);
            return false;
        }
    }

    /// <summary>
    /// Generate corridors for the rooms that still don't have them.
    /// </summary>
    /// <param name="hangingRooms">Rooms without corridors going out of them.</param>
    /// <returns>Every corridor generated.</returns>
    private Queue<Corridor> GenerateCorridors(Queue<Room> hangingRooms)
    {
        if (debugLevel >= DebugLevel.MINIMAL) Debug.Log("<b>Creating corridors</b>");

        Queue<Corridor> corridors = new Queue<Corridor>(hangingRooms.Count); //assume at least one corridor per room at first

        //until every room has a corridor going out of it, make corridors
        while (hangingRooms.Count > 0)
        {
            Rect room = hangingRooms.Dequeue().Bounds;

            Rect rightCorridor;
            if (CarveCorridor(room, Vector2.right, out rightCorridor))
                corridors.Enqueue(new Corridor(rightCorridor, Vector2.right));

            Rect bottomCorridor;
            if (CarveCorridor(room, Vector2.up, out bottomCorridor))
                corridors.Enqueue(new Corridor(bottomCorridor, Vector2.up));
        }

        return corridors;
    }

    /// <summary>
    /// Gets a random room cell within the given bounds.
    /// </summary>
    /// <param name="roomBounds">Area of the room.</param>
    /// <param name="maxTries">Arbitrary value to avoid the searc taking too long.</param>
    /// <param name="cell">Coordinates of the random cell found.</param>
    /// <returns>True if a cell was found as intended.</returns>
    public bool GetRandomRoomCell(Rect roomBounds, int maxTries, out Vector2 cell)
    {
        cell = Vector2.zero;

        do
        {
            cell = new Vector2((int)Random.Range(roomBounds.xMin, roomBounds.xMax), (int)Random.Range(roomBounds.yMin, roomBounds.yMax));
        } while (_dungeon[(int)cell.y, (int)cell.x] != CellType.room && --maxTries > 0);

        return maxTries > 0;
    }

    /// <summary>
    /// Gets a cell located at the edge of a given room.
    /// </summary>
    /// <param name="roomBounds">Area of the room.</param>
    /// <param name="direction">Which side (right or bottom) to search for the cell.</param>
    /// <returns>Coordinates of the cell found.</returns>
    public Vector2 GetRoomEdgeCell(Rect roomBounds, Vector2 direction)
    {
        if (debugLevel >= DebugLevel.MINIMAL) Debug.Log("Getting edge cell of room:" + roomBounds + " with direction = " + direction);
        Vector2 cell = roomBounds.min;

        if (direction.x == 0)           //the cell should be at the right (last column) of the room
            cell.x = roomBounds.xMax - 1;
        else                            //the cell should be at the bottom (last row) of the room
            cell.y = roomBounds.yMax - 1;

        try
        {
            while (cell.x < roomBounds.xMax && cell.y < roomBounds.yMax && _dungeon[(int)cell.y, (int)cell.x] != CellType.room)
            {
                cell += direction;
            }
        } catch (System.IndexOutOfRangeException ex)
        {
            throw new System.IndexOutOfRangeException("Went out of bounds while searching for a cell at the " + (direction.x == 0 ? "right" : "bottom") + " edge of room: " + roomBounds + " at " + cell, ex);
        }
        return cell;
    }

    /// <summary>
    /// Creates a corridor.
    /// </summary>
    /// <param name="roomBounds">Position and dimension of the room where the corridor shall start.</param>
    /// <param name="direction">The direction (right or bottom) of the corridor.</param>
    /// <param name="corridor">Bounds of the corridor created.</param>
    /// <returns>True if a corridor was created without any intersections.</returns>
    private bool CarveCorridor(Rect roomBounds, Vector2 direction, out Rect corridor)
    {
        if(debugLevel >= DebugLevel.IN_DEPTH) Debug.Log("<color=#2a2>CarveCorridor(" + roomBounds + ", " + direction + ",  out Rect corridor)</color>");

        corridor = new Rect();

        if (direction.y != 0 //if the corridor is vertical
            && roomBounds.yMax >= _dungeonHeight - _minimumCorridorLength) //but there isn't space for the smalles corridor allowed below the given room
        {
            return false;
        }
        else if (direction.x != 0 //if the corridor is horizontal
            && roomBounds.xMax >= _dungeonWidth - _minimumCorridorLength) //but there isn't space for the smalles corridor allowed to the right of the given room
        {
            return false;
        }

        Vector2 corridorStart;

        if (debugLevel >= DebugLevel.IN_DEPTH) Debug.Log("Trying to get random room cell - " + roomBounds);

        if(!GetRandomRoomCell(roomBounds, (int)(roomBounds.width * roomBounds.height), out corridorStart))
        {
            if (debugLevel >= DebugLevel.IN_DEPTH) Debug.Log("Random search failed - falling back to linear search.");

            corridorStart = GetRoomEdgeCell(roomBounds, direction);
        }

        //move the corridor starting point outside the room
        while (corridorStart.x < _dungeonWidth && corridorStart.y < _dungeonHeight && _dungeon[(int)corridorStart.y, (int)corridorStart.x] != CellType.wall)
            corridorStart += direction;

        if (debugLevel >= DebugLevel.IN_DEPTH) Debug.Log("Corridor start: " + corridorStart);

        corridor.position = corridorStart;

        //if there would be no space for the smallest room after making a corridor with the minimum length, no use creating one
        if (    (direction.x != 0 && corridor.x + _minimumRoomWidth >= _dungeonWidth)
            ||  (direction.y != 0 && corridor.y + _minimumRoomHeight >= _dungeonHeight))
            return false;

        //move the end of the corridor to the very edge of the room bounds (on the direction the corridor should go)
        while ((direction.x != 0 && corridor.xMax < roomBounds.xMax) || (direction.y != 0 && corridor.yMax < roomBounds.yMax))
            corridor.max += direction;

        corridor.max += new Vector2(Random.Range(_minimumCorridorLength, _maximumCorridorLength + 1) * direction.x,
                                    Random.Range(_minimumCorridorLength, _maximumCorridorLength + 1) * direction.y);

        //if direction.x == 0, corridorEnd.x will be 0, to add direction.y (1) to it (the same applies for the y coordinates)
        corridor.xMax += direction.y;
        corridor.yMax += direction.x;

        //reduce the corridor until it is too small or until a room can fit at it's end
        while (corridor.min != corridor.max
                && ((corridor.xMax + _minimumRoomWidth) * direction.x > _dungeonWidth
                || (corridor.yMax + _minimumRoomHeight) * direction.y > _dungeonHeight))
            corridor.max -= direction;

        if (corridor.size.x == 0 && corridor.size.y == 0)
        {
            Debug.LogWarning("No use creating a corridor with 0 length (" + roomBounds +")");
            return false;
        }
        if(debugLevel >= DebugLevel.IN_DEPTH)
        {
            Debug.Log("Intended corridor end: " + corridor.max);
            Debug.Log("Intended corridor: " + corridor);
        }

        Vector2 predefinedSize = corridor.size;

        corridor.max = MarkCells(corridor.position, corridor.size, CellType.corridor, false);

        if (debugLevel >= DebugLevel.IN_DEPTH) Debug.Log("<color=#22f>Carved corridor = " + corridor + "</color> -> returning " + (predefinedSize == corridor.size));

        return predefinedSize == corridor.size;
    }

    /// <summary>
    /// Creates a room with a random shape.
    /// </summary>
    /// <param name="corridorEnd">Starting point for the generation of the room.</param>
    /// <param name="maxSize">Maximum width and height the room could possibly have.</param>
    /// <param name="offset">If the room can go above (y != 0) or to the left of (x != 0) the starting coordinates</param>
    /// <returns>True if the room was created.</returns>
    private bool CarveRoom(Vector2 corridorEnd, Vector2 minSize, Vector2 maxSize, Vector2 offset, out Room r)
    {
        if(RoomFactory.CarveRoom(this, corridorEnd, minSize, maxSize, offset, _maximumTries, out r))
        {
            foreach (Vector2 v in r.Cells)
                _dungeon[(int)v.y, (int)v.x] = CellType.room;
            return true;
        }
        return false;
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
        for(int row = (int)pos.y; row < _dungeonHeight && row < pos.y + size.y; row++)
        {
            for (int col = (int)pos.x; col < _dungeonWidth && col < pos.x + size.x; col++)
            {
                if (_dungeon[row, col] == type)
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

                if(y >= 0 && y < _dungeon.GetLength(0)
                    && x >= 0 && x < _dungeon.GetLength(1)
                    && _dungeon[y, x] == CellType.wall)
                {
                    //if this cell is within the dungeon and have at least one unmarked cell adjacent to it, then it is at the edge of a room
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
        for (int row = (int)start.y; row < (int)(start.y + size.y) && row < _dungeonHeight; row++)
        {
            for (int col = (int)start.x; col < (int)(start.x + size.x) && col < _dungeonWidth; col++)
            {
                if (_dungeon[row, col] != CellType.wall && !ignoreIntersection)
                    return new Vector2(col, row);

                _dungeon[row, col] = type;
                
            }
        }

        if (debugLevel >= DebugLevel.COMPLETE) DisplayDungeon();

        return start+size;
    }
}
