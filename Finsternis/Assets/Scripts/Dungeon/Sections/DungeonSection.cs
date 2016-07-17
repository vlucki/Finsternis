using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finsternis
{
    public abstract class DungeonSection : ScriptableObject, IEnumerable<Vector2>
    {
        protected Rect bounds;
        protected HashSet<DungeonSection> connections;

        public HashSet<DungeonSection> Connections { get { return connections; } }
        public Dictionary<Vector2, DungeonFeature> features;

        public Vector2 Size { get { return bounds.size; } }
        public float Width { get { return bounds.width; } }
        public float Height { get { return bounds.height; } }
        public float X { get { return bounds.x; } }
        public float Y { get { return bounds.y; } }

        public virtual Vector2 Position
        {
            get { return bounds.position; }
            set { bounds.position = value; }
        }
        public virtual Rect Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        public static T CreateInstance<T>(Rect bounds) where T : DungeonSection
        {
            T section = CreateInstance<T>();
            section.Bounds = bounds;
            return section;
        }

        protected DungeonSection() : this(new Rect()) { }

        protected DungeonSection(Rect bounds)
        {
            this.bounds = bounds;
            connections = new HashSet<DungeonSection>();
            features = new Dictionary<Vector2, DungeonFeature>();
        }

        public abstract bool ContainsCell(Vector2 cell);

        public void AddConnection(DungeonSection connection, bool updateNewConennection = false)
        {
            connections.Add(connection);
            if (updateNewConennection)
                connection.AddConnection(this);
        }

        public void RemoveConnection(DungeonSection connection)
        {
            connections.Remove(connection);
        }

        public abstract IEnumerator<Vector2> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddFeature(DungeonFeature feature, Vector2 cell)
        {
            if (!features.ContainsKey(cell))
            {
                features.Add(cell, feature);
            }
        }

        public virtual DungeonFeature GetFeature(Vector2 cell)
        {
            DungeonFeature feature = null;
            features.TryGetValue(cell, out feature);
            return feature;
        }

        public virtual DungeonFeature GetFeature(float cellX, float cellY)
        {
            return GetFeature(new Vector2(cellX, cellY));
        }

        public abstract bool AddCell(Vector2 cell);

        public abstract void RemoveCell(Vector2 cell);
    }
}