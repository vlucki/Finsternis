using System.Collections.Generic;
using UnityEngine;


public class SimpleDungeon : Dungeon
{

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

    [SerializeField]
    [Tooltip("How many times should it try to carve a room before stoping")]
    [Range(1, 1000000)]
    private int maximumTries = 500000;

    [SerializeField]
    [Header("Minimum room dimensions")]
    [Range(3, 1000000)]
    private int minimumRoomWidth = 3;

    [SerializeField]
    [Range(3, 1000000)]
    private int minimumRoomHeight = 3;

    [SerializeField]
    [Header("Maximum room dimensions")]
    [Range(3, 1000000)]
    private int maximumRoomWidth = 7;

    [SerializeField]
    [Range(3, 1000000)]
    private int maximumRoomHeight = 7;

    public override void Generate()
    {
        Debug.Log("Generating dungeon");
        base.Generate();

        _dungeon = new CellType[dungeonHeight, dungeonWidth];
        Queue<Rect> hangingCorridors = null;
        Queue<Rect> hangingRooms = new Queue<Rect>();
        
        Vector2 maximumRoomSize = new Vector2(maximumRoomWidth, maximumRoomHeight);
        Vector2 minimumRoomSize = new Vector2(minimumRoomWidth, minimumRoomHeight);
        Vector2 offset = Vector2.zero;
        
        int roomCount = 0;
        while (roomCount < totalRooms)
        {
            if(roomCount == 0) //carve the very first room
            {
                Debug.Log("Creating first room.");
                hangingRooms.Enqueue(CarveRoom(Vector2.zero, minimumRoomSize, maximumRoomSize, offset));
                Debug.Log("Finished creating first room.");
                roomCount++;
            }
            else
            {
                Debug.Log("Creating rooms.");
                roomCount += GenerateRooms(hangingRooms, hangingCorridors, minimumRoomSize, maximumRoomSize, ref offset);
                Debug.Log("Finished creating rooms.");
            }

            Debug.Log("Creating corridors.");
            hangingCorridors = GenerateCorridors(hangingRooms);
            Debug.Log("Finished creating corridors.");

            Debug.Log("Dungeon generated.");
        }
    }

    /// <summary>
    /// Generate a room for every hanging corridor (corridors with no room attatched to it's end).
    /// </summary>
    /// <param name="hangingRooms">Rooms generated.</param>
    /// <param name="hangingCorridors">Corridors that need rooms attatched to their end.</param>
    /// <param name="minimumRoomSize">Minimum width and height for the rooms.</param>
    /// <param name="maximumRoomSize">Maximum width and height for the rooms.</param>
    /// <param name="offset">If the room is to the right or below the corridor.</param>
    /// <returns>How many rooms were created.</returns>
    public int GenerateRooms(Queue<Rect> hangingRooms, Queue<Rect> hangingCorridors, Vector2 minimumRoomSize, Vector2 maximumRoomSize, ref Vector2 offset)
    {
        int roomCount = 0;
        //until there are no hanging corridors (that is, corridors with rooms only at their start) 
        while (hangingCorridors.Count > 0)
        {
            //make a room at the end of a corridor or at the top left of the dungeon (if it is empty)
            //and add it to the queue of rooms without corridors
            hangingRooms.Enqueue(CarveRoom(hangingCorridors.Dequeue().max, minimumRoomSize, maximumRoomSize, offset));

            //if this is the first room, the next one will be to its right
            if (offset == Vector2.zero)
                offset = Vector2.right;
            else //if not, then change the offset (if the last one was a room to the right, the next will be below and vice-versa)
            {
                Vector2 newOffset = new Vector2(offset.y, offset.x);
                offset = newOffset;
            }

            roomCount++;
        }
        return roomCount;
    }

    /// <summary>
    /// Generate corridors for the rooms that still don't have them.
    /// </summary>
    /// <param name="hangingRooms">Rooms without corridors going out of them.</param>
    /// <returns>Every corridor generated.</returns>
    private Queue<Rect> GenerateCorridors(Queue<Rect> hangingRooms)
    {
        Queue<Rect> corridors = new Queue<Rect>(hangingRooms.Count); //assume at least one corridor per room at first

        //until every room has a corridor going out of it, make corridors
        while (hangingRooms.Count > 0)
        {
            Rect room = hangingRooms.Dequeue();

            Rect rightCorridor;
            if (CarveCorridor(room, Vector2.right, out rightCorridor))
                corridors.Enqueue(rightCorridor);

            Rect bottomCorridor;
            if (CarveCorridor(room, Vector2.up, out bottomCorridor))
                corridors.Enqueue(bottomCorridor);
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
            cell = new Vector2(Random.Range(roomBounds.xMin, roomBounds.xMax), Random.Range(roomBounds.yMin, roomBounds.yMax));
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
    /// <returns>True if a corridor was created.</returns>
    private bool CarveCorridor(Rect roomBounds, Vector2 direction, out Rect corridor)
    {
        Debug.Log("CarveCorridor(" + roomBounds + ", " + direction + ",  out Rect corridor)");

        corridor = new Rect();

        if (direction.y != 0 //if the corridor is vertical
            && roomBounds.yMax >= _dungeon.GetLength(0) - 1) //but there isn't space for a small (1 cell) corridor below the given room
        {
            return false;
        }
        else if (direction.x != 0 //if the corridor is horizontal
            && roomBounds.xMax >= _dungeon.GetLength(1) - 1) //but there isn't space for a small (1 cell) corridor to the right of the given room
        {
            return false;
        }

        Vector2 corridorStart;

        Debug.Log("Trying to get random room cell - " + roomBounds);
        if(!GetRandomRoomCell(roomBounds, 10000, out corridorStart))
        {
            Debug.Log("Random search failed - falling back to linear search.");
            corridorStart = GetRoomEdgeCell(roomBounds, direction);
        }
        Debug.Log("Corridor start: " + corridorStart);

        corridor.position = corridorStart;

        //move the end of the corridor to the very edge of the room (on the direction the corridor should go)
        Vector2 minimumCorridorEnd = corridor.position + new Vector2((roomBounds.xMax - corridor.x) * direction.x, (roomBounds.yMax - corridor.y) * direction.y);

        Vector2 corridorEnd = minimumCorridorEnd + new Vector2(Random.Range(1, 10) * direction.x, Random.Range(1, 10) * direction.y);

        Debug.Log("Intended corridor end: " + corridorEnd);

        corridor.max = corridorEnd;

        corridor.max = MarkCells(corridor.position, corridor.size, CellType.corridor, false);

        Debug.Log("Actual corridor end: " + corridor.max);

        return true;
    }

    /// <summary>
    /// Creates a room with a random shape.
    /// </summary>
    /// <param name="startingPos">Starting point for the generation of the room.</param>
    /// <param name="maxSize">Maximum width and height the room could possibly have.</param>
    /// <param name="offset">If the room can go above (y != 0) or to the left of (x != 0) the starting coordinates</param>
    private Rect CarveRoom(Vector2 startingPos, Vector2 minSize, Vector2 maxSize, Vector2 offset)
    {
        Debug.Log("CarveRoom(" + startingPos + ", " + minSize + ", " + maxSize + ", " + offset + ")");
        Vector2 pos = startingPos;
        
        Rect roomBounds = new Rect(startingPos, Vector2.zero);

        //keeps marking cells at random locations within the room, until a certain percentage is reached or until the maximum tries is reached
        for (int tries = 0; tries < maximumTries; tries++)
        {
            //make sure a segment with the given dimensions won't go over the room bounds
            Vector2 size = new Vector2();
            size.x = (int)Random.Range(minSize.x, maxSize.x - pos.x + startingPos.x);
            size.y = (int)Random.Range(minSize.y, maxSize.y - pos.y + startingPos.y);

            //if the room is too small, no point in creating it
            if (size.x == 0 || size.y == 0)
                continue;

            if (pos.x < roomBounds.x)
                roomBounds.x = pos.x;
            if (pos.y < roomBounds.y)
                roomBounds.y = pos.y;

            if (pos.x + size.x > roomBounds.xMax)
                roomBounds.xMax = pos.x + size.x;
            if (pos.y + size.y > roomBounds.yMax)
                roomBounds.yMax = pos.y + size.y;


            MarkCells(pos, size);

            pos.x += Random.Range(0, size.x / 2) * (Random.Range(0, 2) == 0 ? -1 : 0); //add some horizontal offset based off of the last calculated width
            pos.y += Random.Range(0, size.y / 2) * (Random.Range(0, 2) == 0 ? -1 : 0); //add some vertical offset based off of the last calculated height

            //ensure the new coordinates are withing the dungeon
            if (pos.x < 0)
                pos.x = 0;
            else if(pos.x >= _dungeon.GetLength(1))
                pos.x = _dungeon.GetLength(1) - size.x;

            if (pos.y < 0)
                pos.y = 0;
            else if (pos.y >= _dungeon.GetLength(0))
                pos.y = _dungeon.GetLength(0) - size.y;

            //and that they are not too further left or above what they could 
            if (offset.x == 0 && pos.x < startingPos.x)
                pos.x = (int)startingPos.x;
            if (offset.y == 0 && pos.y < startingPos.y)
                pos.y = (int)startingPos.y;
        }

        Debug.Log("Carved room bounds: " + roomBounds);

        return roomBounds;
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
    /// <returns>The point where there was an intersection.</returns>
    private Vector2 MarkCells(Vector2 start, Vector2 size, CellType type = CellType.room, bool ignoreIntersection = true)
    {
        Vector2 stoppingPoint = Vector2.zero;
        for (int row = (int)start.y; row < (int)(start.y + size.y) && row < _dungeon.GetLength(0); row++)
        {
            for (int col = (int)start.x; col < (int)(start.x + size.x) && col < _dungeon.GetLength(1); col++)
            {
                if (_dungeon[row, col] != CellType.empty && !ignoreIntersection)
                    return stoppingPoint;

                _dungeon[row, col] = type;

                stoppingPoint.x++;
            }
            stoppingPoint.y++;
        }

        return stoppingPoint;
    }
}
