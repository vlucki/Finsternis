namespace Finsternis
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityQuery;

    public abstract class DungeonSection : ScriptableObject, IEnumerable<Vector2>
    {
        private DungeonSectionTheme theme;
        protected Rect bounds;
        protected HashSet<DungeonSection> connections;

        public HashSet<DungeonSection> Connections { get { return connections; } }
        private Dictionary<Vector2, List<DungeonFeature>> features;
        private List<Vector2> edgeCells;

        public Dungeon Dungeon { get; protected set; }
        public Vector2 Size { get { return bounds.size; } }
        public float Width { get { return bounds.width; } }
        public float Height { get { return bounds.height; } }
        public float X { get { return bounds.x; } }
        public float Y { get { return bounds.y; } }

        public DungeonSectionTheme Theme { get { return this.theme; } }

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

        public virtual void SetTheme<T>(T theme) where T : DungeonSectionTheme { this.theme = theme; }
        public T GetTheme<T>() where T : DungeonSectionTheme { return (T)this.theme; }

        public static T CreateInstance<T>(Rect bounds, Dungeon dungeon) where T : DungeonSection
        {
            T section = CreateInstance<T>();
            section.Dungeon = dungeon;
            section.Bounds = bounds;
            return section;
        }

        protected DungeonSection() : this(new Rect()) { }

        protected DungeonSection(Rect bounds)
        {
            this.bounds = bounds;            
            connections = new HashSet<DungeonSection>();
            features = new Dictionary<Vector2, List<DungeonFeature>>();
        }

        public void AddConnection(DungeonSection connection, bool updateNewConennection = false)
        {
            if (connections.Contains(connection))
                return;

            connections.Add(connection);
            if (updateNewConennection)
                connection.AddConnection(this);
        }

        public void RemoveConnection(DungeonSection connection)
        {
            connections.Remove(connection);
        }

        /// <summary>
        /// Adds a feature to this section at a given position.
        /// </summary>
        /// <param name="feature">Feature to be added.</param>
        /// <param name="cell">Coordinates of the feature to be added.</param>
        /// <returns>True if the feature was added.</returns>
        public bool AddFeature(DungeonFeature feature, Vector2 cell)
        {
            List<DungeonFeature> cellFeatures = null;
            if (this.features.TryGetValue(cell, out cellFeatures))
            {
                if (feature.Type != DungeonFeature.FeatureType.REPLACEMENT)
                {
                    if (cellFeatures.Any((f) => !f.MayStackWith(feature)))
                        return false;
                }
            }
            else
            {
                this.features.Add(cell, new List<DungeonFeature>());
                cellFeatures = features[cell];
            }

            if (feature.Type == DungeonFeature.FeatureType.REPLACEMENT)
                cellFeatures.RemoveAll((f) => f.Type == DungeonFeature.FeatureType.REPLACEMENT);

            cellFeatures.Add(feature);
            return true;
        }

        public List<DungeonFeature> GetFeaturesAt(Vector2 cell)
        {
            List<DungeonFeature> features = null;
            this.features.TryGetValue(cell, out features);
            return features;
        }

        public DungeonFeature GetFeatureAt(Vector2 cell)
        {
            DungeonFeature feature = null;
            List<DungeonFeature> features = GetFeaturesAt(cell);
            if (features != null && features.Count > 0)
                    feature = features[0];

            return feature;
        }

        public DungeonFeature GetFeatureAt(float cellX, float cellY)
        {
            return GetFeatureAt(new Vector2(cellX, cellY));
        }

        public bool HasFeature<T>(Vector2 cell) where T : DungeonFeature
        {
            var features = GetFeaturesAt(cell);
            if (features.Any(feature => feature is T))
                return true;

            return false;
        }

        public Vector2 GetRandomEdgeCell()
        {
            if (this.edgeCells.IsNullOrEmpty())
                FindEdgeCells();
            return this.edgeCells.GetRandom(Dungeon.Random.IntRange);
        }

        public void FindEdgeCells()
        {
            this.edgeCells = new List<Vector2>();
            foreach(var cell in this)
            {

                var up = cell.SumY(1);
                bool edgeCellFound = this.Dungeon.IsOfType(up, null);

                if (!edgeCellFound)
                {
                    var down = cell.SumY(-1);
                    edgeCellFound = this.Dungeon.IsOfType(down, null);
                }
                if (!edgeCellFound)
                {
                    var left = cell.SumX(-1);
                    edgeCellFound = this.Dungeon.IsOfType(left, null);
                }
                if (!edgeCellFound)
                {
                    var right = cell.SumX(1);
                    edgeCellFound = this.Dungeon.IsOfType(right, null);
                }

                if(edgeCellFound)
                    edgeCells.Add(cell);

            }
        }

        public virtual bool ContainsAll(params Vector2[] cells)
        {
            foreach (var cell in cells)
                if (!Contains(cell))
                    return false;
            return true;
        }

        public abstract bool AddCell(Vector2 cell);

        public abstract void RemoveCell(Vector2 cell);

        public abstract bool Contains(Vector2 cell);

        public abstract Vector2 GetRandomCell(params int[] constraints);

        public abstract IEnumerator<Vector2> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}