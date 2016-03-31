using System.Collections.Generic;
using UnityEngine;


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

    public enum CellType
    {
        empty = 0,
        room = 10,
        corridor = 20
    }

    private CellType[,] _dungeon;

    [Header("Dungeon dimensions")]
    [SerializeField]
    [Range(10, 1000000)]
    private int dungeonWidth = 50;

    [SerializeField]
    [Range(10, 1000000)]
    private int dungeonHeight = 50;

    [SerializeField]
    [Range(2, 1000)] //at least one starting point and one exit
    private int totalRooms = 5;

    [Tooltip("How many times should it try to carve a room before stoping")]
    [SerializeField]
    [Range(1, 100)]
    private int maximumTries = 3;

    [Header("Minimum room dimensions")]
    [SerializeField]
    [Range(2, 1000000)]
    private int minimumRoomWidth = 3;

    [SerializeField]
    [Range(2, 1000000)]
    private int minimumRoomHeight = 3;

    [Header("Maximum room dimensions")]
    [SerializeField]
    [Range(2, 1000000)]
    private int maximumRoomWidth = 7;

    [SerializeField]
    [Range(2, 1000000)]
    private int maximumRoomHeight = 7;

    [Header("Corridor length")]
    [SerializeField]
    [Range(1, 1000000)]
    private int minimumCorridorLength = 1;

    [SerializeField]
    [Range(1, 1000000)]
    private int maximumCorridorLength = 5;

    public override void Generate()
    {
        Debug.Log("<b>GENERATING DUNGEON</b>");
        base.Generate();

        _dungeon = new CellType[dungeonHeight, dungeonWidth];
        Queue<Corridor> hangingCorridors = null;
        Queue<Rect> hangingRooms = new Queue<Rect>();
        
        Vector2 maximumRoomSize = new Vector2(maximumRoomWidth, maximumRoomHeight);
        Vector2 minimumRoomSize = new Vector2(minimumRoomWidth, minimumRoomHeight);

        int roomCount = 0;
        do
        {

            DisplayDungeon();
            if (roomCount == 0) //carve the very first room
            {
                Debug.Log("<b>Creating first room</b>");
                Rect room;
                if (CarveRoom(Vector2.zero, minimumRoomSize, maximumRoomSize, Vector2.zero, out room))
                    hangingRooms.Enqueue(room);
                else
                    throw new System.InvalidOperationException("Could not create a single room within the dungeon. Maybe it is too small and the room too big?");
                Debug.Log("<b>Finished creating first room</b>");
                roomCount++;
            }
            else
            {

                Debug.Log("<b>Creating corridors</b>");
                hangingCorridors = GenerateCorridors(hangingRooms);
                Debug.Log("<b>Finished creating corridors</b>");

                DisplayDungeon();

                Debug.Log("<b>Creating rooms</b>");
                GenerateRooms(hangingRooms, hangingCorridors, minimumRoomSize, maximumRoomSize, ref roomCount);
                Debug.Log("<b>Finished creating rooms</b>");
            }
            
        } while (roomCount < totalRooms  //keep going until the desired number of rooms was generated
                && (hangingRooms.Count > 0 || hangingCorridors.Count > 0)); //or until no more rooms or corridors can be generated

        ConnectLeftoverCorridors(hangingCorridors);

        Debug.Log("<b>DUNGEON GENERATED</b>");

        //TODO: debug only code - REMOVE
        DisplayDungeon();
    }

    private void ConnectLeftoverCorridors(Queue<Corridor> hangingCorridors)
    {
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

    private void TrimCorridor(Corridor corridor)
    {
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
                        || (col + 1 < dungeonWidth && _dungeon[row, col + 1] == CellType.corridor)) //there is a corridor do the right
                    {
                        intersectionFound = true;
                    }
                }
                else if ((row > 0 && _dungeon[row - 1, col] == CellType.corridor) //there is a corridor above
                        || (row + 1 < dungeonHeight && _dungeon[row + 1, col] == CellType.corridor)) //there if a corridor below
                {
                    intersectionFound = true;
                }
                trimEnd.x = col;
            }
            trimEnd.y = row;
        }

        if(trimEnd != bounds.max)
        {
            MarkCells(trimEnd, bounds.max, CellType.empty);
        }
    }

    //TODO: debug only code - REMOVE
    private void DisplayDungeon()
    {
        string dungeon = "";
        for(int row = -1; row < dungeonHeight; row++)
        {
            if (row >= 0)
                dungeon += (row > 9 ? "" : "0") + row;
            else
                dungeon += "---";

            for (int col = 0; col < dungeonWidth; col++)
            {
                dungeon += "|";

                if (row < 0)
                {
                    dungeon += (col % 2 != 0? " " : "") + (col > 9 ? "" : "0") + col;
                }
                else
                {
                    switch (_dungeon[row, col])
                    {
                        case CellType.empty:
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

    /// <summary>
    /// Generate a room for every hanging corridor (corridors with no room attatched to it's end).
    /// </summary>
    /// <param name="hangingRooms">Rooms generated.</param>
    /// <param name="hangingCorridors">Corridors that need rooms attatched to their end.</param>
    /// <param name="minimumRoomSize">Minimum width and height for the rooms.</param>
    /// <param name="maximumRoomSize">Maximum width and height for the rooms.</param>
    /// <returns>How many rooms were created.</returns>
    public void GenerateRooms(Queue<Rect> hangingRooms, Queue<Corridor> hangingCorridors, Vector2 minimumRoomSize, Vector2 maximumRoomSize, ref int roomCount)
    {
        //until there are no hanging corridors (that is, corridors with rooms only at their start) 
        while (hangingCorridors.Count > 0 && roomCount < totalRooms)
        {
            //make a room at the end of a corridor and add it to the queue of rooms without corridors
            Rect room;
            Corridor corridor = hangingCorridors.Dequeue();

            if (CarveRoom(corridor.Bounds.max, minimumRoomSize, maximumRoomSize, corridor.Direction, out room))
            {
                hangingRooms.Enqueue(room);
            }
            else
            {
                ExtendCorridor(corridor);
            }

            roomCount++;
        }
    }

    private bool ExtendCorridor(Corridor corridor)
    {
        Debug.Log("<b>Extending corridor: </b>" + corridor);
        Vector2 newCorridorEnd = corridor.Bounds.max;

        while(newCorridorEnd.x < dungeonWidth 
            && newCorridorEnd.y < dungeonHeight
            && _dungeon[(int)(newCorridorEnd.y - corridor.Direction.x), (int)(newCorridorEnd.x - corridor.Direction.y)] == CellType.empty)
        {
            newCorridorEnd += corridor.Direction;
        }

        if (newCorridorEnd != corridor.Bounds.max
            && newCorridorEnd.x < dungeonWidth
            && newCorridorEnd.y < dungeonHeight
            && _dungeon[(int)(newCorridorEnd.y - corridor.Direction.x), (int)(newCorridorEnd.x - corridor.Direction.y)] != CellType.empty)
        {
            corridor.Bounds = new Rect(corridor.Bounds.x, corridor.Bounds.y, newCorridorEnd.x - corridor.Bounds.xMin, newCorridorEnd.y - corridor.Bounds.yMin);
            Debug.Log("Corridor successfully extended - " + corridor);
            MarkCells(corridor.Bounds.position, corridor.Bounds.size, CellType.corridor);
            return true;
        }
        else
        {
            Debug.Log("Impossible to extend the corridor from " + corridor.Bounds.max + " - stopped at " + newCorridorEnd);
            return false;
        }
    }

    /// <summary>
    /// Generate corridors for the rooms that still don't have them.
    /// </summary>
    /// <param name="hangingRooms">Rooms without corridors going out of them.</param>
    /// <returns>Every corridor generated.</returns>
    private Queue<Corridor> GenerateCorridors(Queue<Rect> hangingRooms)
    {
        Queue<Corridor> corridors = new Queue<Corridor>(hangingRooms.Count); //assume at least one corridor per room at first

        //until every room has a corridor going out of it, make corridors
        while (hangingRooms.Count > 0)
        {
            Rect room = hangingRooms.Dequeue();

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
        Vector2 cell = roomBounds.min;

        if (direction.x != 0)           //the cell should be at the right (last column) of the room
            cell.x = roomBounds.xMax;
        else                            //the cell should be at the bottom (last row) of the room
            cell.y = roomBounds.yMax;

        while (_dungeon[(int)cell.y, (int)cell.x] != CellType.room && cell.x <= roomBounds.xMax && cell.y <= roomBounds.yMax){
            cell += direction;
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
        Debug.Log("<color=#2a2>CarveCorridor(" + roomBounds + ", " + direction + ",  out Rect corridor)</color>");

        corridor = new Rect();

        if (direction.y != 0 //if the corridor is vertical
            && roomBounds.yMax >= dungeonHeight - 1) //but there isn't space for a small (1 cell) corridor below the given room
        {
            return false;
        }
        else if (direction.x != 0 //if the corridor is horizontal
            && roomBounds.xMax >= dungeonWidth - 1) //but there isn't space for a small (1 cell) corridor to the right of the given room
        {
            return false;
        }

        Vector2 corridorStart;

        Debug.Log("Trying to get random room cell - " + roomBounds);
        if(!GetRandomRoomCell(roomBounds, (int)(roomBounds.width * roomBounds.height), out corridorStart))
        {
            Debug.Log("Random search failed - falling back to linear search.");
            corridorStart = GetRoomEdgeCell(roomBounds, direction);
        }

        //move the corridor starting point outside the room
        while (corridorStart.x < dungeonWidth && corridorStart.y < dungeonHeight && _dungeon[(int)corridorStart.y, (int)corridorStart.x] != CellType.empty)
            corridorStart += direction;

        Debug.Log("Corridor start: " + corridorStart);

        corridor.position = corridorStart;

        //if there would be no space for the smallest room after making a corridor with the minimum length, no use creating one
        if (    (direction.x != 0 && corridor.x + minimumRoomWidth >= dungeonWidth)
            ||  (direction.y != 0 && corridor.y + minimumRoomHeight >= dungeonHeight))
            return false;

        //move the end of the corridor to the very edge of the room bounds (on the direction the corridor should go)
        while ((direction.x != 0 && corridor.xMax < roomBounds.xMax) || (direction.y != 0 && corridor.yMax < roomBounds.yMax))
            corridor.max += direction;

        corridor.max += new Vector2(Random.Range(minimumCorridorLength, maximumCorridorLength + 1) * direction.x,
                                    Random.Range(minimumCorridorLength, maximumCorridorLength + 1) * direction.y);

        //if direction.x == 0, corridorEnd.x will be 0, to add direction.y (1) to it (the same applies for the y coordinates)
        corridor.xMax += direction.y;
        corridor.yMax += direction.x;

        //reduce the corridor until it is too small or until a room can fit at it's end
        while (corridor.min != corridor.max
                && ((corridor.xMax + minimumRoomWidth) * direction.x > dungeonWidth
                || (corridor.yMax + minimumRoomHeight) * direction.y > dungeonHeight))
            corridor.max -= direction;

        if (corridor.size.x == 0 && corridor.size.y == 0)
        {
            Debug.Log("No use creating a corridor with 0 length.");
            return false;
        }

        Debug.Log("Intended corridor end: " + corridor.max);
        Debug.Log("Intended corridor: " + corridor);

        Vector2 predefinedSize = corridor.size;

        corridor.max = MarkCells(corridor.position, corridor.size, CellType.corridor, false);        

        Debug.Log("<color=#22f>Carved corridor = " + corridor + "</color> -> returning " + (predefinedSize == corridor.size));

        return predefinedSize == corridor.size;
    }

    /// <summary>
    /// Creates a room with a random shape.
    /// </summary>
    /// <param name="corridorEnd">Starting point for the generation of the room.</param>
    /// <param name="maxSize">Maximum width and height the room could possibly have.</param>
    /// <param name="offset">If the room can go above (y != 0) or to the left of (x != 0) the starting coordinates</param>
    /// <returns>True if the room was created.</returns>
    private bool CarveRoom(Vector2 corridorEnd, Vector2 minSize, Vector2 maxSize, Vector2 offset, out Rect roomBounds)
    {
        Debug.Log("<color=#f22>CarveRoom(" + corridorEnd + ", " + minSize + ", " + maxSize + ", " + offset + ")</color>");
        
        Vector2 pos = corridorEnd;
        pos.x -= offset.y;
        pos.y -= offset.x;
        roomBounds = new Rect(pos, Vector2.zero);

        //TODO: do a better checking in order to fit rooms
        //if there is apparently no horizontal space for the room
        if (pos.x + minSize.x > dungeonWidth)
        {
            if (offset.x == 0) //and the corridor it's connected to is above it
                pos.x = dungeonWidth - minSize.x; //move the room left
            else
            {
                Debug.Log("<color=#fff>Impossible to fit a room at " + pos + " due to the dungeon's width.</color>");
                return false;
            }
        }

        //if there is apparently no vertical space for the room
        if (pos.y + minSize.y > dungeonHeight)
        {
            if(offset.y == 0) //and the corridor it's connected to is to its left
                pos.y = dungeonHeight - minSize.y; //move the room up
            else
            {
                Debug.Log("<color=#fff>Impossible to fit a room at " + pos + " due to the dungeon's height.</color>");
                return false;
            }
        }

        bool enoughSpaceForRoom = !SearchInArea(pos, minSize, CellType.corridor);

        while (!enoughSpaceForRoom //if the room is currently intersecting a corridor
                && ((offset.y != 0 && pos.x + minSize.x >= roomBounds.x) //and it can be moved to the left
                || (offset.x != 0 && pos.y + minSize.y >= roomBounds.y)))//or up
        {
            Debug.Log("<color=#522>Room at " + pos + " intersects with a corridor.</color>");
            //move the room and check again
            pos.x -= offset.y;
            pos.y -= offset.x;
            enoughSpaceForRoom = !SearchInArea(pos, minSize, CellType.corridor);
        } 

        if (!enoughSpaceForRoom) //if a room with the minimum size possible would still intersect a corridor, stop trying to make it
        {
            Debug.Log("<color=#fff>Impossible to fit a room at " + pos + " due to an intersection with a corridor.</color>");
            return false;
        }
        bool roomCarved = false;
        //mark cells at random locations within the room, until the maximum tries is reached
        for (int tries = 0; tries < maximumTries; tries++)
        {
            //make sure a segment with the given dimensions won't go over the room bounds
            Vector2 size = new Vector2();
            size.x = (int)Random.Range(minSize.x, maxSize.x - pos.x + corridorEnd.x + 1);
            size.y = (int)Random.Range(minSize.y, maxSize.y - pos.y + corridorEnd.y + 1);

            Debug.Log("Try #" + (tries + 1) + ": position = " + pos + ", size = " + size + ", room = " + roomBounds);

            if (SearchInArea(pos, size, CellType.corridor))
                continue;
            
            if (pos.x < roomBounds.x)
                roomBounds.x = pos.x;
            if (pos.y < roomBounds.y)
                roomBounds.y = pos.y;

            if (pos.x + size.x > roomBounds.xMax)
                roomBounds.xMax = pos.x + size.x;
            if (pos.y + size.y > roomBounds.yMax)
                roomBounds.yMax = pos.y + size.y;

            Debug.Log("new room = " + roomBounds);

            MarkCells(pos, size);

            pos.x += Random.Range(0, size.x / 2) * (Random.Range(0, 2) == 0 ? -1 : 0); //add some horizontal offset based off of the last calculated width
            pos.y += Random.Range(0, size.y / 2) * (Random.Range(0, 2) == 0 ? -1 : 0); //add some vertical offset based off of the last calculated height

            //ensure the new coordinates are withing the dungeon
            if (pos.x < 0)
                pos.x = 0;
            else if(pos.x >= dungeonWidth)
                pos.x = dungeonWidth - size.x;

            if (pos.y < 0)
                pos.y = 0;
            else if (pos.y >= dungeonHeight)
                pos.y = dungeonHeight - size.y;

            //and that they are not too further left or above what they could 
            if (offset.x == 0 && pos.x < corridorEnd.x - 1)
                pos.x = (int)corridorEnd.x - 1;
            if (offset.y == 0 && pos.y < corridorEnd.y - 1)
                pos.y = (int)corridorEnd.y - 1;

            roomCarved = true;
        }

        if (roomBounds.xMax > dungeonWidth)
            roomBounds.xMax = dungeonWidth;

        if (roomBounds.yMax > dungeonHeight)
            roomBounds.yMax = dungeonHeight;

        if (roomCarved)
            Debug.Log("<color=purple>Carved room: " + roomBounds + "</color>");
        else
            Debug.Log("<color=purple>Failed to carve room</color>");

        return roomCarved;
    }

    /// <summary>
    /// Searches a given area for a given cell type;
    /// </summary>
    /// <param name="pos">The area starting point.</param>
    /// <param name="size">The width and height of the search area.</param>
    /// <param name="type">The type of cell that is being searched.</param>
    /// <returns></returns>
    private bool SearchInArea(Vector2 pos, Vector2 size, CellType type)
    {
        for(int row = (int)pos.y; row < dungeonHeight && row < pos.y + size.y; row++)
        {
            for (int col = (int)pos.x; col < dungeonWidth && col < pos.x + size.x; col++)
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
                    && _dungeon[y, x] == CellType.empty)
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
        for (int row = (int)start.y; row < (int)(start.y + size.y) && row < dungeonHeight; row++)
        {
            for (int col = (int)start.x; col < (int)(start.x + size.x) && col < dungeonWidth; col++)
            {
                if (_dungeon[row, col] != CellType.empty && !ignoreIntersection)
                    return new Vector2(col, row);

                _dungeon[row, col] = type;
                
            }
        }

        return start+size;
    }
}
