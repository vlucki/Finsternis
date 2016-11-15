namespace Finsternis
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityQuery;

    public class Corridor : DungeonSection
    {
        private Vector2 direction;
        private int length;

        public Vector2 Direction { get { return this.direction; } set { this.direction = value; } }
        public override Rect Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                this.length = Mathf.RoundToInt(Mathf.Max(bounds.size.x, bounds.size.y));
            }
        }

        public int Length
        {
            get { return this.length; }

            set
            {
                if (value > 0)
                {
                    this.length = value;
                    bounds.size = new Vector2(value * this.direction.x + this.direction.y, value * this.direction.y + this.direction.x);
                }
                else
                {
                    this.length = 0;
                    bounds.size = Vector2.zero;
                }
            }
        }

        public Vector2 End
        {
            get { return this.length > 0 ? this[Length - 1] : Position; }
        }

        public Vector2 this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Length)
                {
#if DEBUG
                    Log.E(this, "Trying to access a cell with index {0} within {1}", index, this);
#endif
                    index = Mathf.Clamp(index, 0, this.Length - 1);
                }
                return bounds.position + this.direction * index;
            }
        }
        
        public new CorridorTheme Theme { get { return base.Theme as CorridorTheme; } }

        public static Corridor CreateInstance(Rect bounds, Vector2 direction, Dungeon dungeon)
        {
            Corridor c = CreateInstance<Corridor>(bounds, dungeon);
            c.direction = direction;
            return c;
        }

        public static Corridor CreateInstance(Vector2 position, int length, Vector2 direction, Dungeon dungeon)
        {
            return Corridor.CreateInstance(
                new Rect(position, direction * length + new Vector2(direction.y, direction.x)), 
                direction, dungeon);
        }

        public override string ToString()
        {
            return "Corridor[bounds:" + Bounds + "; direction:" + Direction + "; length: " + this.length + "]";
        }

        public override IEnumerator<Vector2> GetEnumerator()
        {
            for (int i = 0; i < this.length; i++)
                yield return this[i];
        }

        public void UpdateConnections()
        {
            foreach (DungeonSection connection in connections)
                connection.AddConnection(this);
        }

        public void RemoveLast()
        {
            Length--;
        }

        public void RemoveFirst()
        {
            Rect newBounds = new Rect();
            newBounds.position = Position + this.direction;
            newBounds.size = Size - this.direction;
            Bounds = newBounds;
        }

        public Corridor[] RemoveAt(int index)
        {
            if (index < 0 || index >= Length)
                throw new System.ArgumentOutOfRangeException("index", "Value should be in the range 0 " + Length);

            Corridor[] result = new Corridor[2];

            if (index > 0)
            {
                result[0] = CreateInstance(bounds.position, index, this.direction, this.Dungeon);
                result[0].SetTheme(this.Theme);
            }

            if (index < this.length - 1)
            {
                result[1] = CreateInstance(this[index + 1], this.length - index - 1, this.direction, this.Dungeon);
                result[1].SetTheme(this.Theme);
            }

            foreach (DungeonSection connection in connections)
            {
                if (result[0] && connection.Contains(Position - Direction))
                    result[0].AddConnection(connection, true);
                else if (result[1] && connection.Contains(End + Direction))
                    result[1].AddConnection(connection, true);

                connection.RemoveConnection(this);
            }
            return result;
        }

        public override bool Contains(Vector2 cell)
        {
            return cell.x >= X && cell.x <= End.x && cell.y >= Y && cell.y <= End.y;
        }

        /// <summary>
        /// Adds a cells to this corridor, but only immediately before the first or after the last.
        /// </summary>
        /// <param name="cell">The cell to be added.</param>
        /// <returns>True if the cell was added.</returns>
        public override bool AddCell(Vector2 cell)
        {
            if (cell == this[0] - Direction)
            {
                Position -= Direction;
                Length++;
            }
            else if (cell == End + Direction)
                Length++;
            else
                return false;

            return true;

        }

        public override void RemoveCell(Vector2 cell)
        {
            if (Contains(cell))
            {
                while (End != cell)
                    Length--;
            }
        }

        public override Vector2 GetRandomCell(params int[] constraints)
        {
            if (this.Length == 1)
                return this[0];
            else
            {
                int min = 0, max = this.Length;
                if (constraints != null)
                {
                    min = Mathf.Max(constraints[0], 0);
                    if (constraints.Length > 1)
                        max = Mathf.Max(constraints[1], min);
                }
                if (min == max)
                    return this[min];

                return this[UnityEngine.Random.Range(min, max)];
            }
        }

    }
}