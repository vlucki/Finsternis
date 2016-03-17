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

    //Connect the rooms if that is not already the case
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

        //if rooms are diagonal, move on - for now
        if (xDiff != 0 && yDiff != 0)
        {
            return;
        }

        //stores every pair of cells that should be merged
        List<int[]> toMerge = new List<int[]>();

        for (int rowA = startingRowA, rowB = startingRowB; rowA < roomA.Height && rowB < roomB.Height; rowA++, rowB++)
        {
            for (int colA = startingColumnA, colB = startingColumnB; colA < roomA.Width && colB < roomB.Width; colA++, colB++)
            {

                //if the rooms are touching already, no need to keep checking
                if (roomA[rowA, colA] && roomB[rowB, colB])
                {
                    return;
                }

                int[] cellPair = GetNearestCellPair(roomA, roomB, xDiff, yDiff); //x and y of first cell, x and y of second cell and distance between both
            }
        }
    }

    private int[] GetCellNearestToEdge(DungeonRoom room, int xIncrement, int yIncrement)
    {
        int startingCol = xIncrement < 0 ? 0 : room.Width - 1;
        int endingCol = xIncrement < 0 ? room.Width - 1 : 0;
        int startingRow = yIncrement < 0 ? 0 : room.Height - 1;
        int endingRow = yIncrement < 0 ? room.Height - 1 : 0;
        int distanceTravelled = 0;

        //either the outer or the inner loop should ALWAYS run only once
        do
        {
            do
            {
                distanceTravelled++; //add 1 to the distance, since we are moving away from the edge

                if (room[startingRow, startingCol])
                {
                    //found the closest marked cell to the border on this row or column
                    return new int[3] { startingRow, startingCol, distanceTravelled };
                }

                startingCol += xIncrement;

            } while (startingCol != endingCol && xIncrement != 0);

            startingRow += yIncrement;
        } while (startingRow != endingRow && yIncrement != 0);
        
        //finished scanning the whole row or column without finding a marked cell
        return null;
    }

    private int[] GetNearestCellPair(DungeonRoom roomA, DungeonRoom roomB, int xDiff, int yDiff)
    {
        int[] cellPair = new int[5];


        return cellPair;
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