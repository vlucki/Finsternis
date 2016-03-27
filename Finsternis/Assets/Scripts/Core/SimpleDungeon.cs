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


    public void GenerateRooms(Queue<Rect> corridors, Vector2 maxDimensions, Vector2 offset)
    {

    }

    public override void Generate()
    {
        base.Generate();
        Queue<Rect> hangingCorridors = new Queue<Rect>();
        Queue<Rect> hangingRooms = new Queue<Rect>();

        //Start at the center of the dungeon, with a random offset based off of the maximum room dimensions
        Vector2 firstRoomCoordinates = new Vector2((dungeonWidth - Random.Range(0, maximumRoomWidth)) / 2, (dungeonHeight - Random.Range(0, maximumRoomHeight)) / 2);
        Vector2 maximumRoomDimensions = new Vector2(maximumRoomWidth, maximumRoomHeight);
        Vector2 offset = Vector2.zero;

        //TODO: make a graph to hold every room and corridor (rects)
        int roomCount = 0;
        while (roomCount < totalRooms)
        {

            //until there are no hanging corridors (that is, corridors with rooms only at one edge) OR if the dungeon is currently empty
            while(hangingCorridors.Count > 0 || roomCount == 0)
            {
                //make a room at the end of a corridor or at the top left of the dungeon (if it is empty)
                //and add it to the queue of rooms without corridors
                hangingRooms.Enqueue(CarveRoom(roomCount == 0 ? firstRoomCoordinates : hangingCorridors.Dequeue().max, maximumRoomDimensions, offset));

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

            //until every room has a corridor going out of it, make corridors
            while (hangingRooms.Count > 0)
            {
                Rect room = hangingRooms.Dequeue();
                hangingCorridors.Enqueue(CarveCorridor(room, Vector2.right));
                hangingCorridors.Enqueue(CarveCorridor(room, Vector2.up));
            }

            //graph.Add(parentCorridor, room) -> null at the very first iteration
            //graph.Add(room, makeCorridor(room, Vector2.right))
            //graph.Add(room, makeCorridor(room, Vector2.up))



        }

    }

    /// <summary>
    /// Makes a corridor going out from a given room.
    /// </summary>
    /// <param name="roomPerimeter">The cells that surround the room.</param>
    /// <returns>The bounds of the corridor.</returns>
    private Rect CarveCorridor(Rect roomBounds, Vector2 direction)
    {
        Vector2 corridorStart;

        int maxTries = 10000;
        do //get a random cell inside de room
        {
            corridorStart = new Vector2(Random.Range(roomBounds.xMin, roomBounds.xMax), Random.Range(roomBounds.yMin, roomBounds.yMax)); 
        } while (_dungeon[(int)corridorStart.y, (int)corridorStart.x] != CellType.room && --maxTries > 0);

        if(maxTries <= 0) //if the random failed for some reason, do a linear search for a cell at the side of the room
        {
            for(int i = 0; 
                _dungeon[(int)corridorStart.y, (int)corridorStart.x] != CellType.room //stop if a room cell is found
                && i < (roomBounds.width * direction.x) + (roomBounds.height * direction.y); //multiply by direction to get the correct axis, since x or y will always be 0
                i++)
            {
                corridorStart = new Vector2((roomBounds.xMin + i) * direction.x, (roomBounds.yMin + i) * direction.y);
            }
        }
        
        //move the end of the corridor to the very edge of the room (on the direction the corridor should go)
        Vector2 corridorEnd = corridorStart + new Vector2(roomBounds.xMin * direction.x, roomBounds.yMin * direction.y);

        corridorEnd += new Vector2(Random.Range(1, 10) * direction.x, Random.Range(1, 10) * direction.y);

        bool intersection = false; //checks if the corridor intersected a room or another corridor
        //TODO: check if the corridor is not too long
        for(int i = (int)corridorStart.y; !intersection && i < corridorEnd.y; i++)
        {
            for (int j = (int)corridorStart.x; !intersection && j < corridorEnd.x; j++)
            {
                if (_dungeon[i, j] != CellType.empty)
                    _dungeon[i, j] = CellType.corridor;
                else
                    intersection = true;

            }
        }

        return new Rect(corridorStart, corridorEnd - corridorStart);
    }

    /// <summary>
    /// Creates a room with a random shape.
    /// </summary>
    /// <param name="startingCoordinates">Starting point for the generation of the room.</param>
    /// <param name="maxDimensions">Maximum width and height the room could possibly have.</param>
    /// <param name="offset">If the room can go above (y != 0) or to the left of (x != 0) the starting coordinates</param>
    private Rect CarveRoom(Vector2 startingCoordinates, Vector2 maxDimensions, Vector2 offset)
    {
        int maxWidth  = (int)maxDimensions.x;
        int maxHeight = (int)maxDimensions.y;
        int y  = (int)startingCoordinates.y;
        int x  = (int)startingCoordinates.x;

        //TODO: grab the upper left and the dimensions of the room bounds
        Rect roomBounds = new Rect(startingCoordinates, Vector2.zero);

        //keeps marking cells at random locations within the room, until a certain percentage is reached or until the maximum tries is reached
        for (int tries = 0; tries < maximumTries; tries++)
        {
            //make sure a segment with the given dimensions won't go over the room bounds
            int w = (int)Random.Range(0, maxWidth - x + startingCoordinates.x); 
            int h = (int)Random.Range(0, maxHeight - y + startingCoordinates.y);

            if (w == 0 || h == 0)
                continue;

            if (x < roomBounds.x)
                roomBounds.x = x;
            if (y < roomBounds.y)
                roomBounds.y = y;

            if (x + w > roomBounds.xMax)
                roomBounds.xMax = x + w;
            if (y + h > roomBounds.yMax)
                roomBounds.yMax = y + h;


            MarkCells(y, x, w, h);

            x += Random.Range(0, w / 2) * (Random.Range(0, 2) == 0 ? -1 : 0); //add some horizontal offset based off of the last calculated width
            y += Random.Range(0, h / 2) * (Random.Range(0, 2) == 0 ? -1 : 0); //add some vertical offset based off of the last calculated height

            //ensure the new coordinates are withing the dungeon
            if (x < 0)
                x = 0;
            else if(x >= _dungeon.GetLength(1))
                x = _dungeon.GetLength(1) - w;

            if (y < 0)
                y = 0;
            else if (y >= _dungeon.GetLength(0))
                y = _dungeon.GetLength(0) - h;

            //and that they are not too further left or above what they could 
            if (offset.x == 0 && x < startingCoordinates.x)
                x = (int)startingCoordinates.x;
            if (offset.y == 0 && y < startingCoordinates.y)
                y = (int)startingCoordinates.y;
        }

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
    /// Marks every cell on the given room.
    /// </summary>
    /// <param name="startingRow">Row where the marking of cells will start.</param>
    /// <param name="startingCol">Column where the marking of cells will start.</param>
    /// <param name="width">How many cells to mark horizontally.</param>
    /// <param name="height">How many cells to mark vertically.</param>
    /// <param name="type">Type to use when marking the cells.</param>
    private void MarkCells(int startingRow, int startingCol, int width, int height, CellType type = CellType.room)
    {
        for (int row = startingRow; row < startingRow + height && row < _dungeon.GetLength(0); row++)
        {
            for (int col = startingCol; col < startingCol + width && col < _dungeon.GetLength(1); col++)
            {
                _dungeon[row, col] = type;
            }
        }
    }
}
