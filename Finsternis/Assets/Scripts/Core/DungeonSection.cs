using System;
using System.Collections.Generic;
using UnityEngine;
using Rdm = UnityEngine.Random;

internal class DungeonSection : ScriptableObject
{

    private HashSet<DungeonRoom> _rooms;

    private struct CellInfo
    {
        private int row;
        private int column;
        private int distanceFromEdge;

        public int Row { get { return row; } }
        public int Column { get { return column; } }
        public int DistanceFromEdge { get { return distanceFromEdge; } }

        public CellInfo(int row, int column, int distanceFromEdge)
        {
            this.row = row;
            this.column = column;
            this.distanceFromEdge = distanceFromEdge;
        }
    }

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

    //Unify the "rooms" of this section and another
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

    //Returns true only if both rooms are different and are not further than 1 unit from each other on both axis
    private bool CheckAdjacency(DungeonRoom roomA, DungeonRoom roomB)
    {
        return !(roomA.Equals(roomB))
                && (Mathf.Abs(roomA.X - roomB.X) < 2)
                && (Mathf.Abs(roomA.Y - roomB.Y) < 2);
    }

    //Connect the rooms horizontally or vertically
    internal void ConnectRooms(DungeonRoom roomA, DungeonRoom roomB)
    {
        int xDiff = roomA.X - roomB.X;
        int yDiff = roomA.Y - roomB.Y;

        //if rooms are diagonal, move on - for now
        if (xDiff != 0 && yDiff != 0) return;

        int rowA = 0, rowB = 0;
        int colA = 0, colB = 0;

        if (xDiff == -1)        { colA = roomA.Width - 1; } //B is to the right of A
        else if (xDiff == 1)    { colB = roomB.Width - 1; } //B is to the left of A

        if (yDiff == -1)        { rowA = roomA.Height - 1; } //B is below A
        else if (yDiff == 1)    { rowB = roomB.Height - 1; } //B is above A

        //stores every pair of cells that should be merged
        List<CellInfo[]> toMerge = new List<CellInfo[]>();

        while(rowA < roomA.Height && rowB < roomB.Height)
        {
            while (colA < roomA.Width && colB < roomB.Width)
            {
                //if the rooms are touching already, no need to keep checking
                if (roomA[rowA, colA] && roomB[rowA, colB])
                {
                    return;
                }

                CellInfo[] cellPair = GetNearestCellPair(roomA, roomB, xDiff, yDiff); //x, y and distance from border of first cell followed by the same info for second cell

                if (cellPair == null) //nothing to do here
                    continue;

                int distanceBetweenPair = cellPair[0].DistanceFromEdge + cellPair[1].DistanceFromEdge;
                //empty the list if the newly found cells are closer to each other
                if (toMerge.Count != 0 && toMerge[0][0].DistanceFromEdge + toMerge[0][1].DistanceFromEdge > distanceBetweenPair)
                {
                    toMerge.Clear();
                }

                //add the new cells if none were found yet or if they are as distant from each other as the ones found previously
                if (toMerge.Count == 0 || toMerge[0][0].DistanceFromEdge + toMerge[0][1].DistanceFromEdge == distanceBetweenPair)
                {
                    toMerge.Add(cellPair);
                }

                colA++;
                colB++;
            }
            rowA++;
            rowB++;
        }

        //make a line connecting every cell pair
        foreach(CellInfo[] cells in toMerge)
        {
            ExtendCell(cells[0], roomA, xDiff, yDiff);
            ExtendCell(cells[1], roomB, xDiff, yDiff);
        }
    }

    private void ExtendCell(CellInfo cell, DungeonRoom room, int xDiff, int yDiff)
    {
        int w = 1;
        int h = 1;
        int x = cell.Column;
        int y = cell.Column;

        if(xDiff < 0)
        {
            w = cell.Column;
            x = 0;
        }
        else if (xDiff > 0)
        { w = room.Width - cell.Column; }

        if (yDiff > 0)
        {
            h = cell.Row;
            y = 0;
        }
        else if (yDiff < 0)
        { h = room.Height - cell.Row; }

        MarkCells(x, y, w, h, room);
    }

    //tries to get the nearest cells on two rooms that are row-aligned (if both rooms are side by side) or column-aligned (if the rooms are on top of each other)
    private CellInfo[] GetNearestCellPair(DungeonRoom roomA, DungeonRoom roomB, int xDiff, int yDiff)
    {
        CellInfo? cellA = GetCellNearestToEdge(roomA, xDiff, yDiff);

        if (cellA == null)
            return null;

        CellInfo? cellB = GetCellNearestToEdge(roomB, -xDiff, -yDiff);

        if (cellB == null)
            return null;

        return new CellInfo[] { (CellInfo)cellA, (CellInfo)cellB };
    }

    //iterates through every column in a row or every row in a column, looking for a marked cell within the room
    private CellInfo? GetCellNearestToEdge(DungeonRoom room, int xIncrement, int yIncrement)
    {
        //determines whether the loop should be left-right or right-left
        int firstCol =  xIncrement < 0 ? room.Width - 1 : 0;
        int lastCol =   xIncrement > 0 ? room.Width - 1 : 0;

        int firstRow =  yIncrement < 0 ? room.Height - 1 : 0;
        int lastRow =   yIncrement > 0 ? room.Height - 1 : 0;

        int distanceTravelled = 0;

        //either the outer or the inner loop should ALWAYS run only once
        do { do {
                if (room[firstRow, firstCol])
                {
                    //found the closest marked cell to the border on this row or column
                    return new CellInfo(firstRow, firstCol, distanceTravelled);
                }

                distanceTravelled++; //add 1 to the distance, since we are moving away from the edge
                firstCol += xIncrement;

            } while (firstCol != lastCol + xIncrement); //keep looping through every column (or stop immediately if iterating only through rows)

            firstRow += yIncrement;
        } while (firstRow != lastRow + yIncrement);  //keep looping through every row (or stop immediately if iterating only through columns)

        //finished scanning the whole row or column without finding a marked cell
        return null;
    }


    //move every room away from the given center
    internal void MoveAwayFrom(int centerY, int centerX, int amountY, int amountX)
    {
        int avgX = 0, avgY = 0;
        foreach (DungeonRoom room in _rooms)
        {
            avgX += room.X;
            avgY += room.Y;
        }

        avgX = Mathf.CeilToInt((float)avgX / _rooms.Count);
        avgY = Mathf.CeilToInt((float)avgY / _rooms.Count);

        float ratioX = (float)avgX / centerX;
        float ratioY = (float)avgY / centerY;

        if (avgX != centerX)
        {
            amountX = Mathf.CeilToInt(amountX * (ratioX - 1));
        }

        if (avgY != centerY)
        {
            amountY = Mathf.CeilToInt(amountY * (ratioY - 1));
        }

        foreach (DungeonRoom room in _rooms)
        {
            room.X += amountX;
            room.Y += amountY;
        }
    }

    //creates a room within this section
    internal void CreateRoom(float fillRate, int row, int col, int width, int height)
    {
        DungeonRoom room = ScriptableObject.CreateInstance<DungeonRoom>();
        room.Init(width, height, row, col);
        _rooms.Add(room);

        CarveRoom(fillRate, room);
    }

    //Shapes up the given room, making it take up at least a certain percentage (fillRate) of its total area
    private void CarveRoom(float fillRate, DungeonRoom room)
    {
        int width = room.Width;
        int height = room.Height;
        float filled = 0;

        //keeps marking cells at random locations within the room, until a certain percentage is reached or until the maximum tries is reached
        for (int tries = 0; tries < 10000 && filled / (width * height) < fillRate; tries++)
        {
            int x = Rdm.Range(0, width);
            int y = Rdm.Range(0, height);
            int w = Rdm.Range(0, width - x);
            int h = Rdm.Range(0, height - y);

            filled += MarkCells(y, x, w, h, room);
        }
    }

    private int MarkCells(int startingRow, int startingCol, int width, int height, DungeonRoom room)
    {
        int markedCells = 0;
        for (int row = startingRow; row < startingRow + height && row < room.Height; row++)
        {
            for (int col = startingCol; col < startingCol + width && col < room.Width; col++)
            {
                if (room.MarkCell(row, col))
                {
                    markedCells++;
                }
            }
        }

        return markedCells;
    }
}