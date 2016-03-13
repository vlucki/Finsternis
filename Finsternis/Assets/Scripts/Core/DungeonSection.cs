using System;
using System.Collections.Generic;
using UnityEngine;
using Rdm = UnityEngine.Random;

internal class DungeonSection : ScriptableObject
{

    public enum CellType
    {
        UNMARKED = 0,
        MARKED = 1,
        WALL = 2
    }

    private SortedDictionary<UnityEngine.Vector2, CellType[,]> _rooms;

    public int RoomsCount { get { return _rooms.Count; } }

    public DungeonSection()
    {
        _rooms = new SortedDictionary<UnityEngine.Vector2, CellType[,]>();
    }

    //Makes a section based off of another
    private DungeonSection(DungeonSection other)
    {
        _rooms = new SortedDictionary<UnityEngine.Vector2, CellType[,]>(other._rooms);
    }

    public static DungeonSection Merge(DungeonSection a, DungeonSection b)
    {
        DungeonSection result = new DungeonSection(a);
        result.Merge(b);
        return result;
    }
    
    //Shorthand for the static Merge function
    public static DungeonSection operator +(DungeonSection left, DungeonSection right)
    {
        return DungeonSection.Merge(left, right);
    }

    //Unify the "rooms" list of this section and another one
    public void Merge(DungeonSection other)
    {
        foreach(UnityEngine.Vector2 key in other._rooms.Keys)
        {
            if (!_rooms.ContainsKey(key))
            {
                _rooms.Add(key, other._rooms[key]);
            }
        }

        EnsureConnectivity(_rooms);
    }

    private void EnsureConnectivity(SortedDictionary<Vector2, CellType[,]> _rooms)
    {
        foreach (UnityEngine.Vector2 roomCoords in _rooms.Keys)
        {
            foreach (UnityEngine.Vector2 otherRoomCoords in _rooms.Keys)
            {
                int xDifference = (int)Mathf.Abs(roomCoords.x - otherRoomCoords.x);
                int yDifference = (int)Mathf.Abs(roomCoords.y - otherRoomCoords.y);

                //if the two rooms are not the same nor diagonal from each other, but still are adjacent
                if ((xDifference != 0 || yDifference != 0) && xDifference != yDifference && xDifference < 2 && yDifference < 2)
                {
                    if (roomCoords.x < otherRoomCoords.x) //make sure the right of the room at "coords" connects to the left of the room at "other coords"
                    {

                    } else if(roomCoords.x > otherRoomCoords.x) //make sure the left of the room at "coords" connects to the right of the room at "other coords"
                    {

                    } else if(roomCoords.y < otherRoomCoords.y) //make sure the bottom of the room at "coords" connects to the top of the room at "other coords"
                    {

                    } else if(roomCoords.y > otherRoomCoords.y) //make sure the top of the room at "coords" connects to the bottom of the room at "other coords"
                    {

                    }
                }
            }
        }
    }

    //creates a room within this section
    internal void CreateRoom(float fillRate, int col, int row, int areaWidht, int areaHeight)
    {
        CellType[,] room = new CellType[areaWidht, areaHeight];
        UnityEngine.Vector2 coords = new UnityEngine.Vector2(col, row);

        _rooms.Add(coords, room);

        CarveRoom(fillRate, room);
    }

    private void CarveRoom(float fillRate, CellType[,] cells)
    {
        int Width = cells.GetLength(0);
        int Height = cells.GetLength(1);
        float filled = 0;

        for (int tries = 0; tries < 10000 && filled / (Width * Height) < fillRate; tries++)
        {
            int x = Rdm.Range(0, Width);
            int y = Rdm.Range(0, Height);
            int w = Rdm.Range(0, Width - x);
            int h = Rdm.Range(0, Height - y);

            filled += MarkCells(x, y, w, h, cells);
        }
    }

    private int MarkCells(int x, int y, int w, int h, CellType[,] room)
    {
        int markedCells = 0;
        for (int row = y; row < y + h; row++)
        {
            for (int col = x; col < x + w; col++)
            {
                if (room[row, col] == CellType.UNMARKED)
                {
                    room[row, col] = CellType.MARKED;
                    markedCells++;
                }
            }
        }

        return markedCells;
    }
}