using UnityEngine;
using System.Collections;

public class DungeonRoom : ScriptableObject
{

    public enum CellType
    {
        UNMARKED = 0,
        MARKED = 1
    }

    CellType[,] cells;

    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get { return cells.GetLength(1); } }
    public int Height { get { return cells.GetLength(0); } }

    public bool this[int row, int col]
    {
        get { return IsMarked(row, col); }
    }

    public bool Initialized
    {
        get { return cells != null; }
    }

    //Since directly instantiating a ScriptableObject is not recomended, this Init method is used
    public void Init(int width, int height, int y = 0, int x = 0)
    {
        X = x;
        Y = y;
        cells = new CellType[height, width];
    }

    internal void InitializationCheck()
    {
        if (!Initialized)
            throw new System.InvalidOperationException("Tried to execute a method without initializing object");
    }

    //Makes sure a cell is marked and return whether it was already marked or not
    public bool MarkCell(int row, int col)
    {
        InitializationCheck();

        if (!IsMarked(col, row))
        {
            cells[row, col] = CellType.MARKED;
            return true;
        }
        return false;
    }

    public bool IsMarked(int row, int col)
    {
        InitializationCheck();

        return cells[row, col] == CellType.MARKED;
    }

}
