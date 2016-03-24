using System;
using System.Collections.Generic;
using UnityEngine;

internal class DungeonSection : ScriptableObject
{

    private HashSet<DungeonRoom> _rooms;
    private DungeonRandom _random;

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

        public override string ToString()
        {
            return "CellInfo (column: " + column + ", row: " + row + ", distance: " + distanceFromEdge + ") -" + base.ToString();
        }
    }

    public int RoomsCount { get { return _rooms.Count; } }

    /// <summary>
    /// Initializes the section with a given random generator.
    /// </summary>
    /// <param name="random">The section to be used as a base.</param>
    public void Init(DungeonRandom random)
    {
        _rooms = new HashSet<DungeonRoom>(new DungeonRoomComparer());
        this._random = random;
    }

    /// <summary>
    /// Initializes a section based off of another
    /// </summary>
    /// <param name="other">The section to be used as a base.</param>
    public void Init(DungeonSection other)
    {
        _rooms = new HashSet<DungeonRoom>(other._rooms, new DungeonRoomComparer());
        this._random = other._random;
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
    
    /// <summary>
    /// Adds every room from another section to this one.
    /// </summary>
    /// <param name="other">The section whose rooms shall be merged.</param>
    public void Merge(DungeonSection other)
    {
        foreach(DungeonRoom room in other._rooms)
        {
            if (!_rooms.Contains(room))
            {
                _rooms.Add(room);
            }
        }

        EnsureConnectivity();
    }

    /// <summary>
    /// Makes sure every room in this section is connected - that is, that it is possible to walk to every room from every room.
    /// </summary>
    private void EnsureConnectivity()
    {
        foreach (DungeonRoom roomA in _rooms)
        {
            foreach (DungeonRoom roomB in _rooms)
            {
                if (!CheckAdjacency(roomA, roomB))
                    continue;

                ConnectRooms(roomA, roomB);
            }
        }
    }

    /// <summary>
    /// Verifies whether two rooms are adjacent or not.
    /// </summary>
    /// <param name="roomA">One of the rooms to be checked.</param>
    /// <param name="roomB">One of the rooms to be checked.</param>
    /// <returns>True only if both rooms are different and are not further than 1 unit from each other on both axis</returns>
    private bool CheckAdjacency(DungeonRoom roomA, DungeonRoom roomB)
    {
        return !(roomA.Equals(roomB))
                && (Mathf.Abs(roomA.X - roomB.X) < 2)
                && (Mathf.Abs(roomA.Y - roomB.Y) < 2);
    }
    
    /// <summary>
    /// Connects two rooms, ensuring there is a path of marked cells that goes from one to the other.
    /// </summary>
    /// <param name="roomA">One of the rooms to be connected.</param>
    /// <param name="roomB">One of the rooms to be connected.</param>
    internal void ConnectRooms(DungeonRoom roomA, DungeonRoom roomB)
    {
        Debug.Log("Connecting A: " + roomA + " to B: " + roomB);

        int xDiff = roomA.X - roomB.X;
        int yDiff = roomA.Y - roomB.Y;

        Debug.Log("Offset = (x:" + xDiff + ", y:" + yDiff + ")");

        //if rooms are diagonal, move on - for now
        if (xDiff != 0 && yDiff != 0) return;
        else if(xDiff == 0 && yDiff == 0) throw new ArgumentException("Trying to connect a room to itself - which should not happen!");

        int startingRowA = 0, startingRowB = 0;
        int startingColumnA = 0, startingColumnB = 0;

        if (xDiff == -1)     { startingColumnA = roomA.Width - 1; } //B is to the right of A
        else if (xDiff == 1) { startingColumnB = roomB.Width - 1; } //B is to the left of A

        if (yDiff == -1)     { startingRowA = roomA.Height - 1; } //B is below A
        else if (yDiff == 1) { startingRowB = roomB.Height - 1; } //B is above A

        Debug.Log("startingColumnA: " + startingColumnA + ", startingColumnB: " + startingColumnB);
        Debug.Log("startingRowA: " + startingRowA + ", startingRowB: " + startingRowB);

        //stores every pair of cells that should be merged
        List<CellInfo[]> toConnect = FindConenctionPoints(roomA, roomB, startingColumnA, startingColumnB, startingRowA, startingRowB, xDiff, yDiff);

        ConnectCells(toConnect, roomA, roomB, xDiff, yDiff);
    }

    private List<CellInfo[]> FindConenctionPoints(DungeonRoom roomA, DungeonRoom roomB, int startingColumnA, int startingColumnB, int startingRowA, int startingRowB, int xDiff, int yDiff)
    {
        List<CellInfo[]> toConnect = new List<CellInfo[]>();
        int rowA = startingRowA, rowB = startingRowB;
        int colA = startingColumnA, colB = startingColumnB;

        while (rowA < roomA.Height && rowB < roomB.Height && colA < roomA.Width && colB < roomB.Width)
        {
            while (colA < roomA.Width && colB < roomB.Width)
            {
                //if the rooms are touching already, no need to keep checking
                if (roomA[rowA, colA] && roomB[rowB, colB])
                {
                    return toConnect;
                }

                CellInfo nearestCellInA = GetCellNearestToEdge(roomA, xDiff, yDiff, rowA, colA);

                CellInfo nearestCellInB = GetCellNearestToEdge(roomB, -xDiff, -yDiff, rowB, colB);

                if (!nearestCellInA && !nearestCellInB) //no cells to connect
                {
                    if (UpdateColumns(ref colA, ref colB, startingColumnA, startingColumnB, yDiff))
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                bool resortingToDiagonalMerge = !nearestCellInA || !nearestCellInB; //if any of the cells was not found, try to get the closest diagonal cells

                //brefore actually trying to find diagonal cells, check if cells to connect weren't found already
                if (resortingToDiagonalMerge
                    && toConnect.Count > 0)
                {
                    if (UpdateColumns(ref colA, ref colB, startingColumnA, startingColumnB, yDiff))
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
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
                
                if (!nearestCellInA || !nearestCellInB) //no cells to connect, even diagonally
                {
                    if (UpdateColumns(ref colA, ref colB, startingColumnA, startingColumnB, yDiff))
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                int distanceBetweenPair = nearestCellInA.DistanceFromEdge + nearestCellInB.DistanceFromEdge;

                Debug.Log("nearest cell in A: " + nearestCellInA);
                Debug.Log("nearest cell in B: " + nearestCellInB);
                Debug.Log("distance between pair: " + distanceBetweenPair);

                SetupList(toConnect, distanceBetweenPair, resortingToDiagonalMerge);

                //add the new cells if none were found yet or if they are as distant from each other as the ones found previously
                if (toConnect.Count == 0 || toConnect[0][0].DistanceFromEdge + toConnect[0][1].DistanceFromEdge == distanceBetweenPair)
                {
                    toConnect.Add(new CellInfo[2] { nearestCellInA, nearestCellInB });
                }

                if(!UpdateColumns(ref colA, ref colB, startingColumnA, startingColumnB, yDiff))
                {
                    break;
                }
            }

            //if the rows are being checked (column by column), go the next row
            if(yDiff == 0)
            {
                rowA++;
                rowB++;
            }
        }

        return toConnect;
    }

    private bool UpdateColumns(ref int colA, ref int colB, int startingColumnA, int startingColumnB, int yDiff)
    {
        //if the columns are being checked (row by row), go to the next one
        if (yDiff != 0)
        {
            colA++;
            colB++;
            return true;
        }
        else //if not, reset the columns and go to the next row
        {
            colA = startingColumnA;
            colB = startingColumnB;
            return false;
        }
    }

    private void ConnectCells(List<CellInfo[]> toConnect, DungeonRoom roomA, DungeonRoom roomB, int xDiff, int yDiff)
    {
        //make a line connecting every cell pair
        foreach (CellInfo[] cells in toConnect)
        {
            //if the cells are not diagonal, just extend them normally
            if (cells[0].Column == cells[1].Column || cells[0].Row == cells[1].Row)
            {
                ExtendCell(cells[0], roomA, xDiff, yDiff);
                ExtendCell(cells[1], roomB, -xDiff, -yDiff);
            }
            else //otherwise, mark the neighbors of the cells
            {
                MarkNeighbors(cells, roomA, roomB, xDiff, yDiff);
            }
        }
    }

    private void SetupList(List<CellInfo[]> toMerge, int distanceBetweenPair, bool resortingToDiagonalMerge)
    {
        //if the list of cells to be merged is not empty
        if (toMerge.Count != 0)
        {
            //and the cells on the list are further apart from eachother than the newly found pair
            if (toMerge[0][0].DistanceFromEdge + toMerge[0][1].DistanceFromEdge > distanceBetweenPair
                //or the cells on the list are diagonal from each other while the newly found ones aren't
                || (!resortingToDiagonalMerge && toMerge[0][0].Column != toMerge[0][1].Column && toMerge[0][0].Row != toMerge[0][1].Row))
            {
                //throw the previously found cells away
                toMerge.Clear();
            }
        }
    }

    /// <summary>
    /// Mark the cells that are around the given cell pair.
    /// </summary>
    /// <param name="cells">The cell pair to be used as reference.</param>
    /// <param name="roomA">The room from the first cell.</param>
    /// <param name="roomB">The room from the second cell.</param>
    /// <param name="xDiff">Horizontal offset between the rooms.</param>
    /// <param name="yDiff">Vertical offset between the rooms.</param>
    private void MarkNeighbors(CellInfo[] cells, DungeonRoom roomA, DungeonRoom roomB, int xDiff, int yDiff)
    {
        for(int i = -2; i < 3; i ++)
        {
            for(int j = -2; j < 3; j++)
            {
                //Ignore cells that are 2 units away from each other both horizontally and vertically.
                //Also, ignore the cells 0 units away (since they are the ones that were passed to the methdo)
                if (i == j && (i == 0 || Mathf.Abs(i) == 2))
                    continue;

                roomA.MarkCell(cells[0].Row + i, cells[0].Column + j);
                roomB.MarkCell(cells[1].Row + i, cells[1].Column + j);
            }
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
        Debug.Log("Getting nearest cell to edge in " + room);
        Debug.Log("Offset = (x:" + xOffset + ", y:" + yOffset + ")");
        Debug.Log("Starting point = (column:" + col + ", row:" + row + ")");

        //If xOffset is 0, the first and last column will be the same (the rows are the ones that will be changed when checking)
        //The same applies to yOffset and the columns changing while the rows remain the same.

        int firstCol = col;
        int lastCol = col;

        if (xOffset < 0) //check to the left of "col" - so stop at the leftmost cell
        {
            lastCol = 0;
        }
        else if (xOffset > 0) //check to the right of "col" - so end at the rightmost cell
        {
            lastCol = room.Width - 1;
        }

        int firstRow = row;
        int lastRow = row;

        if (yOffset > 0) //check below "row" - so stop at the bottomost cell
        {
            lastRow = room.Height - 1;
        }
        else if (yOffset < 0) //check above "row" - so stop at the topmost cell
        {
            lastRow = 0;
        }

        Debug.Log("firstCol: " + firstCol + ", firstRow: " + firstRow);
        Debug.Log("lastCol: " + lastCol + ", lastRow: " + lastRow);

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
    /// <param name="verticalAmount">The vertical diameter of the "repulsion ellipse".</param>
    /// <param name="horizontalAmount">The horizontal diameter of the "repulsion ellipse".</param>
    internal void MoveAwayFrom(int centerY, int centerX, int verticalAmount, int horizontalAmount)
    {
        Vector2 averageCenter = GetAverageCenter();

        float ratioX = averageCenter.x / centerX;
        float ratioY = averageCenter.y / centerY;

        //if this section is not centered horizontally, calculate how much left of right it should move
        if (averageCenter.x != centerX)
        {
            horizontalAmount = Mathf.CeilToInt(horizontalAmount * (ratioX - 1));
        }

        //if this section is not centered vertically, calculate how much up or down it should move
        if (averageCenter.y != centerY)
        {
            verticalAmount = Mathf.CeilToInt(verticalAmount * (ratioY - 1));
        }

        //move every room whithin this section
        foreach (DungeonRoom room in _rooms)
        {
            room.X += horizontalAmount;
            room.Y += verticalAmount;
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
            int x = _random.Next(0, width);
            int y = _random.Next(0, height);
            int w = _random.Next(0, width - x);
            int h = _random.Next(0, height - y);

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


    //TODO: remove temporaty method!
    internal void DisplayRooms()
    {
        foreach(DungeonRoom room in _rooms)
        {
            room.Display();
        }
    }

}