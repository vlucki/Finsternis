using System;
using System.Collections.Generic;
using UnityEngine;
using Rdm = UnityEngine.Random;

internal class DungeonSection : ScriptableObject
{
    private HashSet<DungeonRoom> _rooms;

    public int RoomsCount { get { return _rooms.Count; } }

    public DungeonSection()
    {
        _rooms = new HashSet<DungeonRoom>(new DungeonRoomComparer());
    }

    //Initializes a section based off of another
    public void Init(DungeonSection other)
    {
        _rooms = new HashSet<DungeonRoom>(other._rooms, new DungeonRoomComparer());
    }

    public static DungeonSection Merge(DungeonSection a, DungeonSection b)
    {
        DungeonSection result = ScriptableObject.CreateInstance<DungeonSection>();
        result.Init(a);
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
        foreach(DungeonRoom room in other._rooms)
        {
            if (!_rooms.Contains(room))
            {
                _rooms.Add(room);
            }
        }

        EnsureConnectivity(_rooms);
    }

    //Returns true only if both rooms are different and are not further than 1 unit from each other on both axis
    private bool CheckAdjacency(DungeonRoom roomA, DungeonRoom roomB)
    {
        return  !(roomA.Equals(roomB)) 
                && (Mathf.Abs(roomA.X - roomB.X) < 2) 
                && (Mathf.Abs(roomA.Y - roomB.Y) < 2);
    }

    private void EnsureConnectivity(HashSet<DungeonRoom> rooms)
    {
        foreach(DungeonRoom roomA in rooms)
        {
            foreach (DungeonRoom roomB in rooms)
            {
                if (!CheckAdjacency(roomA, roomB))
                    continue;
                ConnectRooms(roomA, roomB);
            }
        }
    }

    internal void ConnectRooms(DungeonRoom roomA, DungeonRoom roomB)
    {

        int xDiff = roomA.X - roomB.X;
        int yDiff = roomA.Y - roomB.Y;

        int startingRowA = 0, startingRowB = 0;
        int startingColumnA = 0, startingColumnB = 0;

        if (xDiff == -1)
        {
            //B is to the right of A
            startingColumnA = roomA.Width - 1;
        }
        else if (xDiff == 1)
        {
            //B is to the left of A
            startingColumnB = roomB.Width - 1;
        }
        if (yDiff == -1)
        {
            //B is below A
            startingRowA = roomA.Height - 1;
        }
        else if (yDiff == 1)
        {
            //B is above A
            startingRowB = roomB.Height - 1;
        }

        for (int row = startingRowA; row < roomA.Height; row++)
        {
            for (int col = startingColumnA; col < roomA.Width; col++)
            {

            }
        }
    }

    //creates a room within this section
    internal void CreateRoom(float fillRate, int col, int row, int width, int height)
    {
        DungeonRoom room = ScriptableObject.CreateInstance<DungeonRoom>();
        room.Init(width, height, col, row);
        _rooms.Add(room);

        CarveRoom(fillRate, room);
    }

    private void CarveRoom(float fillRate, DungeonRoom room)
    {
        int width = room.Width;
        int height = room.Height;
        float filled = 0;

        for (int tries = 0; tries < 10000 && filled / (width * height) < fillRate; tries++)
        {
            int x = Rdm.Range(0, width);
            int y = Rdm.Range(0, height);
            int w = Rdm.Range(0, width - x);
            int h = Rdm.Range(0, height - y);

            filled += MarkCells(x, y, w, h, room);
        }
    }

    private int MarkCells(int x, int y, int w, int h, DungeonRoom room)
    {
        int markedCells = 0;
        for (int row = y; row < y + h; row++)
        {
            for (int col = x; col < x + w; col++)
            {
                if (room.MarkCell(col, row))
                {
                    markedCells++;
                }
            }
        }

        return markedCells;
    }
}