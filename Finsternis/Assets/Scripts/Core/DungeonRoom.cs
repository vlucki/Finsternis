using UnityEngine;
using System.Collections;

public class DungeonRoom : ScriptableObject
{

    /// <summary>
    /// A marked cell is inside a room and can be walked on.
    /// </summary>
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

    /// <summary>
    /// Shorthand for the IsMarked(row, col) method.
    /// </summary>
    /// <param name="row">The row of the cell.</param>
    /// <param name="col">The column of the cell.</param>
    /// <returns>True if the cell is marked.</returns>
    public bool this[int row, int col]
    {
        get { return IsMarked(row, col); }
    }

    public bool Initialized
    {
        get { return cells != null; }
    }

    /// <summary>
    /// Constructor-like method, used to initialize variables.
    /// </summary>
    /// <param name="width">Number of horizontal cells on the room.</param>
    /// <param name="height">Number of verticall cells on the room.</param>
    /// <param name="y">Row occupied by this room on the dungeon.</param>
    /// <param name="x">Column occupied by this room on the dungeon.</param>
    public void Init(int width, int height, int y = 0, int x = 0)
    {
        X = x;
        Y = y;
        cells = new CellType[height, width];
    }

    /// <summary>
    /// Guarantees that the room was properly initialized.
    /// </summary>
    internal void InitializationCheck()
    {
        if (!Initialized)
            throw new System.InvalidOperationException("Tried to execute a method without initializing object");
    }
    
    /// <summary>
    /// Marks a cell, if it wasn't already.
    /// </summary>
    /// <param name="row">The row of the cell.</param>
    /// <param name="col">The column of the cell.</param>
    /// <returns>True if the cell was not already marked.</returns>
    public bool MarkCell(int row, int col)
    {
        InitializationCheck();

        if (!this[row, col])
        {
            cells[row, col] = CellType.MARKED;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Verifies if a cell is already marked.
    /// </summary>
    /// <param name="row">The row of the cell.</param>
    /// <param name="col">The column of the cell.</param>
    /// <returns>True if the cell is already marked.</returns>
    public bool IsMarked(int row, int col)
    {
        InitializationCheck();

        return cells[row, col] == CellType.MARKED;
    }

}
