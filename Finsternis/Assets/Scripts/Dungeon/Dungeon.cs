using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Finsternis
{

    public class DungeonRandom : MTRandom, IRandom
    {
        public DungeonRandom() : base()
        {
        }

        public DungeonRandom(int seed) : base(seed) { }

        public void SetSeed(int seed)
        {
            this._rand.init((uint)seed);
        }

        public void SetSeed(string seed)
        {
            SetSeed(seed.GetHashCode());
        }
    }

    public class Dungeon : MonoBehaviour
    {
        private static IRandom random;

        public static IRandom Random
        {
            get
            {
                if (random == null)
                    random = new DungeonRandom();
                return random;
            }
        }

        [SerializeField]
        private int seed;

        [SerializeField]
        private bool customSeed = true;

        [SerializeField]
        private Vector2 entrance;

        [SerializeField]
        private Vector2 exit;

        [SerializeField]
        private List<DungeonGoal> goals;

        private int availableCardPoints = -1;

        private List<Room> rooms;
        private List<Corridor> corridors;
        private DungeonSection[,] dungeonGrid;

        private int remainingGoals;

        public int RemainingGoals { get { return this.remainingGoals; } }

        public int AvailableCardPoints { get { return this.availableCardPoints; } }

        public void AddPoints(int points)
        {
            if (points <= 0)
                return;

            this.availableCardPoints += points;
        }

        public bool UsePoints(int points)
        {
            if (points >= 0 || this.availableCardPoints < points)
                return false;

            this.availableCardPoints -= points;
            return true;
        }

        public int Seed
        {
            get { return this.seed; }
            set
            {
                if (customSeed)
                {
                    random = new DungeonRandom(value);
                    this.seed = value;
                }
            }
        }

        public Vector2 Entrance { get { return entrance; } set { entrance = value; } }
        public Vector2 Exit { get { return exit; } set { exit = value; } }

        public int Width { get { return this.dungeonGrid.GetLength(0); } }
        public int Height { get { return this.dungeonGrid.GetLength(1); } }
        public Vector2 Size { get { return new Vector2(Width, Height); } }

        public DungeonSection this[int x, int y]
        {
            get
            {
                try
                {
                    return this.dungeonGrid[x, y];
                }
                catch (IndexOutOfRangeException ex)
                {
                    throw new IndexOutOfRangeException("Attempting to access a cell outside of dungeon! [" + x + ";" + y + "]", ex);
                }
            }
            set
            {
                try
                {
                    this.dungeonGrid[x, y] = value;
                }
                catch (IndexOutOfRangeException ex)
                {
                    throw new IndexOutOfRangeException("Attempting to access a cell outside of dungeon! [" + x + ";" + y + "]", ex);
                }
            }
        }

        public DungeonSection this[float x, float y]
        {
            get { return this[(int)x, (int)y]; }
            set { this[(int)x, (int)y] = value; }
        }

        public DungeonSection this[Vector2 pos]
        {
            get { return this[(int)pos.x, (int)pos.y]; }
            set { this[(int)pos.x, (int)pos.y] = value; }
        }

        public List<Corridor> Corridors { get { return corridors; } }
        public List<Room> Rooms { get { return rooms; } }

        public UnityEvent OnGoalCleared;

        public void Init(int width, int height)
        {
            if (Dungeon.Random == null)
            {
                if (customSeed)
                    random = new DungeonRandom(this.seed);
                else
                    random = new DungeonRandom();
            }

            this.dungeonGrid = new DungeonSection[width, height];
            this.corridors = new List<Corridor>();
            this.rooms = new List<Room>();
            this.goals = new List<DungeonGoal>();
            if (OnGoalCleared == null)
                OnGoalCleared = new UnityEvent();
        }

        public T GetGoal<T>() where T : DungeonGoal
        {
            foreach (DungeonGoal goal in this.goals)
            {
                if (goal is T)
                    return (T)goal;
            }
            return null;
        }

        public T[] GetGoals<T>() where T : DungeonGoal
        {
            List<T> goals = new List<T>();
            this.goals.ForEach((goal) => { if (goal is T) goals.Add(goal as T); });
            return goals.ToArray();
        }

        public T AddGoal<T>() where T : DungeonGoal
        {
            T goal = gameObject.AddComponent<T>();
            this.goals.Add(goal);
            goal.onGoalReached.AddListener(
                (g) =>
                {
                    remainingGoals--;
                    OnGoalCleared.Invoke();
                });

            remainingGoals++;
            return goal;
        }

        public Room GetRandomRoom()
        {
            return this.rooms[Random.IntRange(0, this.rooms.Count, false)];
        }

        /// <summary>
        /// Checks if a given set of coordinates is within the dungeon bounds.
        /// </summary>
        /// <param name="cell">The coordinates to be checked.</param>
        /// <returns>True if the given cell is within the dungeon bounds.</returns>
        public bool IsWithinDungeon(Vector2 cell)
        {
            return IsWithinDungeon(cell.x, cell.y);
        }

        /// <summary>
        /// Checks if a given set of coordinates is within the dungeon bounds.
        /// </summary>
        /// <param name="x">The column to check.</param>
        /// <param name="y">The row to check.</param>
        /// <returns>True if both X and Y are within the dungeon bounds.</returns>
        public bool IsWithinDungeon(float x, float y)
        {
            return x >= 0
                && x < Width
                && y >= 0
                && y < Height;
        }

        /// <summary>
        /// Searches a given area for the given cell types;
        /// </summary>
        /// <param name="pos">The area starting point.</param>
        /// <param name="size">The width and height of the search area.</param>
        /// <param name="types">The types of cell that are being searched.</param>
        /// <returns>True if any of the types was found</returns>
        public bool SearchInArea(Vector2 pos, Vector2 size, params Type[] types)
        {
            if (types == null || types.Length < 1)
                throw new InvalidOperationException("Must provide at least one type to be searched for.");

            for (int row = (int)pos.y; row < Height && row < pos.y + size.y; row++)
            {
                for (int col = (int)pos.x; col < Width && col < pos.x + size.x; col++)
                {
                    Vector2 cell = new Vector2(col, row);
                    if (IsWithinDungeon(cell) && IsOfAnyType(cell, types))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Searches around a given coordinate for the provided tyes.
        /// </summary>
        /// <param name="pos">The coordinate whose surroundings will be checked.</param>
        /// <param name="requiredNumberOfMatches">How many matches should happen before stopping the check?</param>
        /// <param name="checkDiagonals">Should diagonals be chekced? (ie. [x-1, y-1])</param>
        /// <param name="types">The types to be considered.</param>
        /// <returns>How many cells of the provided types were found.</returns>
        public int SearchAround(Vector2 pos, int requiredNumberOfMatches, bool checkDiagonals, params Type[] types)
        {
            if (types == null || types.Length < 1)
                throw new InvalidOperationException("Must provide at least one type to be searched for.");

            int cellsFound = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == j || (!checkDiagonals && Mathf.Abs(i) == Mathf.Abs(j)))
                        continue;

                    Vector2 neighbourCell = pos + new Vector2(i, j);
                    if (IsWithinDungeon(neighbourCell) && IsOfAnyType(neighbourCell, types))
                        cellsFound++;
                    if (requiredNumberOfMatches > 0 && cellsFound == requiredNumberOfMatches)
                        return cellsFound;
                }
            }

            return cellsFound;
        }

        /// <summary>
        /// Checks if a given rectangle overlaps a corridor
        /// </summary>
        /// <param name="pos">Upper left corner of the rectangle.</param>
        /// <param name="size">Dimenstions of the rectangle.</param>
        /// <returns>True if it does overlap.</returns>
        internal bool OverlapsCorridor(Vector2 pos, Vector2 size)
        {
            Rect r = new Rect(pos, size);

            foreach (Corridor c in corridors)
            {
                if (c.Bounds.Overlaps(r))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if there is an instance of any of the provided types at a given coordinate.
        /// </summary>
        /// <param name="cell">The coordinates to be checked.</param>
        /// <param name="types">The types to be considered.</param>
        /// <returns>True if there is a match between one of the provided types and the object at the provided coordinates.</returns>
        public bool IsOfAnyType(Vector2 cell, params Type[] types)
        {
            foreach (Type type in types)
            {
                if (IsOfType(cell, type))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if there is an instance of the provided type at a given coordinate.
        /// </summary>
        /// <param name="cellX">The column to be checked.</param>
        /// <param name="cellY">The row to be checked.</param>
        /// <param name="type">The type to be considered.</param>
        /// <returns>True if there is a match between the provided type and the type of the object at the provided coordinates.</returns>
        public bool IsOfType(float cellX, float cellY, Type type)
        {
            if (this[cellX, cellY])
            {
                if (this[cellX, cellY].GetType().Equals(type))
                    return true;
            }
            else if (type == null) //Wall
                return true;

            return false;
        }

        /// <summary>
        /// Checks if there is an instance of the provided type at a given coordinate.
        /// </summary>
        /// <param name="cell">The coordinates to be checked.</param>
        /// <param name="type">The type to be considered.</param>
        /// <returns>True if there is a match between the provided type and the type of the object at the provided coordinates.</returns>
        public bool IsOfType(Vector2 cell, Type type)
        {
            return IsOfType(cell.x, cell.y, type);
        }

        /// <summary>
        /// Checks if there is an instance of the provided type at a given coordinate.
        /// </summary>
        /// <typeparam name="T">The type to be considered.</typeparam>
        /// <param name="cell">The coordinates to be checked.</param>
        /// <returns>True if there is a match between the provided type and the type of the object at the provided coordinates.</returns>
        public bool IsOfType<T>(Vector2 cell)
        {
            return IsOfType(cell.x, cell.y, typeof(T));
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

                    if (IsWithinDungeon(x, y)
                        && this[x, y] == null)
                    {
                        return true;
                    }

                }
            }

            return false;
        }

        public void MarkCells(DungeonSection section)
        {
            foreach (Vector2 cell in section)
            {
                this[cell] = section;
            }
        }
    }
}