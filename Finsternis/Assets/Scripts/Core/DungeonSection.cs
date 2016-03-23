using System;
using System.Collections.Generic;
using UnityEngine;
using Rdm = UnityEngine.Random;

internal class DungeonSection : ScriptableObject
{

    private HashSet<DungeonRoom> _rooms;

    private class CellInfo
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

        public static implicit operator bool(CellInfo info)
        {
            return info != null;
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

        if (xDiff == -1)     { colA = roomA.Width - 1; } //B is to the right of A
        else if (xDiff == 1) { colB = roomB.Width - 1; } //B is to the left of A

        if (yDiff == -1)     { rowA = roomA.Height - 1; } //B is below A
        else if (yDiff == 1) { rowB = roomB.Height - 1; } //B is above A

        //stores every pair of cells that should be merged
        List<CellInfo[]> toMerge = new List<CellInfo[]>();

        while (rowA < roomA.Height && rowB < roomB.Height)
        {
            while (colA < roomA.Width && colB < roomB.Width)
            {
                //if the rooms are touching already, no need to keep checking
                if (roomA[rowA, colA] && roomB[rowA, colB])
                {
                    return;
                }

                CellInfo nearestCellInA = GetCellNearestToEdge(roomA, xDiff, yDiff, rowA, colA);

                CellInfo nearestCellInB = GetCellNearestToEdge(roomA, xDiff, yDiff, rowB, colB);

                if (!nearestCellInA && !nearestCellInB) //nothing to do here
                    continue;

                bool resortingToDiagonalMerge = !nearestCellInA || !nearestCellInB; //if any of the cells was not found, try to get the closest diagonal cells

                //brefore actually trying to find diagonal cells, check if cells to connect weren't found already
                if(resortingToDiagonalMerge 
                    && toMerge.Count > 0 ){
                    continue;
                }

                //if there is a cell right at the edge of room A, but no corresponding onde was found in B
                if (!nearestCellInB && nearestCellInA.DistanceFromEdge == 0) 
                {
                    nearestCellInB = GetDiagonalCell(roomB, -xDiff, -yDiff, rowB, colB);
                }
                //if there is a cell right at the edge of room B, but no corresponding onde was found in A
                else if (!nearestCellInA && nearestCellInB.DistanceFromEdge == 0)
                {
                    nearestCellInA = GetDiagonalCell(roomA, xDiff, yDiff, rowA, colA);
                }

                int distanceBetweenPair = nearestCellInA.DistanceFromEdge + nearestCellInB.DistanceFromEdge;
                //if the list of cells to be merged is not empty
                if (toMerge.Count != 0)
                {
                    //and the cells on the list are further apart from eachother than the newly found pair
                    if (toMerge[0][0].DistanceFromEdge + toMerge[0][1].DistanceFromEdge > distanceBetweenPair)
                    {
                        toMerge.Clear();
                    }
                }

                //add the new cells if none were found yet or if they are as distant from each other as the ones found previously
                if (toMerge.Count == 0 || toMerge[0][0].DistanceFromEdge + toMerge[0][1].DistanceFromEdge == distanceBetweenPair)
                {
                    toMerge.Add(new CellInfo[2] { nearestCellInA, nearestCellInB });
                }

                colA += xDiff;
                colB -= xDiff; //goes to the opposite side of A
            }
            rowA += yDiff;
            rowB -= yDiff; //goes to the opposite side of A
        }

        //make a line connecting every cell pair
        foreach (CellInfo[] cells in toMerge)
        {
            ExtendCell(cells[0], roomA, xDiff, yDiff);
            ExtendCell(cells[1], roomB, -xDiff, -yDiff);
        }
    }

    //marks every cell on the same row or column as the provided cell
    private void ExtendCell(CellInfo cell, DungeonRoom room, int xDiff, int yDiff)
    {
        //at first, the method doesn't know if it should mark cells horizontally or vertically
        int w = 1;
        int h = 1;
        int x = cell.Column;
        int y = cell.Row;

        if(xDiff < 0) //every cell to the left of "cell" should be marked
        {
            x = 0; //from 0
            w = cell.Column; //to the current cell position
        }
        else if (xDiff > 0) //every cell to the right of "cell" should be marked
        { w = room.Width - cell.Column; } //from the current cell position to the last one in the row

        if (yDiff > 0) //every cell above "cell" should be marked
        {
            y = 0; //from 0
            h = cell.Row; //to the current cell position
        }
        else if (yDiff < 0) //evey cell below "cell" should be marked
        { h = room.Height - cell.Row; } //from the current cell position to the last one in the column

        MarkCells(x, y, w, h, room);
    }

    ///<summary>
    ///Iterates through every column in a row or every row in a column, looking for a marked cell within the room.
    ///</summary>
    ///<param name="room">The room that is being checked.</param>
    ///<param name='xOffset'>Whether the cells will be checked left to right or right to left.</param> 
    ///<param name="yOffset">Whether the cells will be checket from top down or bottom up.</param>
    ///<param name="row">Row being checked.</param>
    ///<param name="col">Column being checked.</param>
    private CellInfo GetCellNearestToEdge(DungeonRoom room, int xOffset, int yOffset, int row, int col)
    {
        //If xOffset is 0, the first and last column will be the same (the rows are the ones that will be changed when checking)
        //The same applies to yOffset and the columns changing while the rows remain the same.

        int firstCol = col;
        int lastCol = col;

        if (xOffset < 0) //check to the left of "col" - so start at the leftmost cell
        {
            firstCol = 0;
        }
        else if (xOffset > 0) //check to the right of "col" - so end at the rightmost cell
        {
            lastCol = room.Width - 1;
        }


        int firstRow = row;
        int lastRow = row;

        if (yOffset > 0) //check below "row" - so start at the topmost cell
        {
            firstRow = 0;
        }
        else if (yOffset < 0) //check above "row" - so stop at the bottommost cell
        {
            lastRow = room.Height - 1;
        }

        int distanceTravelled = 0;

        //either the outer or the inner loop should ALWAYS run only once
        do { do {
                if (room[firstRow, firstCol])
                {
                    //found the closest marked cell to the border on this row or column
                    return new CellInfo(firstRow, firstCol, distanceTravelled);
                }

                distanceTravelled++; //add 1 to the distance, since we are moving away from the edge
                firstCol += xOffset;

            } while (firstCol != lastCol + xOffset); //keep looping through every column (or stop immediately if iterating only through rows)

            firstRow += yOffset;
        } while (firstRow != lastRow + yOffset);  //keep looping through every row (or stop immediately if iterating only through columns)

        //finished scanning the whole row or column without finding a marked cell
        return null;
    }

    /// <summary>
    /// Tries to get a marked cell to the left or right of a given unmarked cell that is on the edge of a room.
    /// Such cell would be diagonal to a marked cell on the edge of an adjacent room.
    /// </summary>
    /// <param name="room">Where the cell is being searched.</param>
    /// <param name="xOffset">Whether this room is to the left (-1) or right (1) from another one.</param>
    /// <param name="yOffset">Whether this room is above (-1) or below (1) another one.</param>
    /// <param name="row">The row of the starting point (the unmarked cell).</param>
    /// <param name="col">The column of the starting point (the unmarked cell).</param>
    /// <returns></returns>
    private CellInfo GetDiagonalCell(DungeonRoom room, int xOffset, int yOffset, int row, int col)
    {
        CellInfo cell = null;
        //try to get a cell on a given room that is right at the edge too, but diagonal from the cell
        int absoluteYOffset = Mathf.Abs(yOffset); //1 if the room being checked is above or below (the other one)
        int absoluteXOffset = Mathf.Abs(xOffset); //1 if the room being checked is to the right or left
        
        //check if the column to the right or the row below is marked
        if (col + absoluteYOffset < room.Width - 1 
            && row + absoluteXOffset < room.Height - 1 
            && room[row + absoluteXOffset, col + absoluteYOffset])
        {
            cell = new CellInfo(row + absoluteXOffset, col + absoluteYOffset, 0);
        }
        //check if the column to the left or the row above is marked
        else if (col - absoluteYOffset >= 0 && room[row, col - absoluteYOffset]
            && row - absoluteXOffset >= 0
            && room[row - absoluteXOffset, col - absoluteYOffset])
        {
            cell = new CellInfo(row - absoluteXOffset, col - absoluteYOffset, 0);
        }
        
        return cell;
    }

    /// <summary>
    /// Pushes every room away from the center of an ellipse.
    /// </summary>
    /// <param name="centerY">The y coordinate of the cell in the center of the "repulsion ellipse".</param>
    /// <param name="centerX">The x coordinate of the cell in the center of the "repulsion ellipse".</param>
    /// <param name="verticalDiameter">The vertical diameter of the "repulsion ellipse".</param>
    /// <param name="horizontalDiameter">The horizontal diameter of the "repulsion ellipse".</param>
    internal void MoveAwayFrom(int centerY, int centerX, int verticalDiameter, int horizontalDiameter)
    {
        Vector2 averageCenter = GetAverageCenter();

        float ratioX = averageCenter.x / centerX;
        float ratioY = averageCenter.y / centerY;

        if (averageCenter.x != centerX)
        {
            horizontalDiameter = Mathf.CeilToInt(horizontalDiameter * (ratioX - 1));
        }

        if (averageCenter.y != centerY)
        {
            verticalDiameter = Mathf.CeilToInt(verticalDiameter * (ratioY - 1));
        }

        foreach (DungeonRoom room in _rooms)
        {
            room.X += horizontalDiameter;
            room.Y += verticalDiameter;
        }
    }

    /// <summary>
    /// Calculates the center of this section, taking in account every room whithin it.
    /// </summary>
    /// <returns>The coordinates of the center of this section.</returns>
    private Vector2 GetAverageCenter()
    {
        Vector2 center = Vector2.zero;

        foreach (DungeonRoom room in _rooms)
        {
            center.x += room.X;
            center.y += room.Y;
        }

        center.x = Mathf.CeilToInt(center.x / _rooms.Count);
        center.y = Mathf.CeilToInt(center.y / _rooms.Count);

        return center;
    }

    /// <summary>
    /// Creates a room whithin this section.
    /// </summary>
    /// <param name="fillRate">The percentage of cells that must be marked before a room can be considered complete.</param>
    /// <param name="row">The y coordinate of the room (in dungeon cells).</param>
    /// <param name="col">The x coordinate of the room (in dungeon cells).</param>
    /// <param name="width">The width of the room (in dungeon cells).</param>
    /// <param name="height">The height of the room (in dungeon cells).</param>
    public void CreateRoom(float fillRate, int row, int col, int width, int height)
    {
        DungeonRoom room = ScriptableObject.CreateInstance<DungeonRoom>();
        room.Init(width, height, row, col);
        _rooms.Add(room);

        CarveRoom(fillRate, room);
    }
    
    /// <summary>
    /// Shapes up the given room.
    /// </summary>
    /// <param name="fillRate">The percentage of cells that must be marked before a room can be considered complete.</param>
    /// <param name="room">The room that is to be carved.</param>
    internal void CarveRoom(float fillRate, DungeonRoom room)
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

    /// <summary>
    /// Marks every cell on the given room.
    /// </summary>
    /// <param name="startingRow">The row where the marking of cells will start.</param>
    /// <param name="startingCol">The column where the marking of cells will start.</param>
    /// <param name="width">How many cells to mark horizontally.</param>
    /// <param name="height">How many cells to mark vertically.</param>
    /// <param name="room">The room whose cells will be marked.</param>
    /// <returns></returns>
    internal int MarkCells(int startingRow, int startingCol, int width, int height, DungeonRoom room)
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