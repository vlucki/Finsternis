namespace Finsternis
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public abstract class DungeonSection : ScriptableObject, IEnumerable<Vector2>
    {
        private DungeonSectionTheme theme;
        protected Rect bounds;
        protected HashSet<DungeonSection> connections;

        public HashSet<DungeonSection> Connections { get { return connections; } }
        private Dictionary<Vector2, List<DungeonFeature>> features;

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

        public bool AddFeature(DungeonFeature feature, Vector2 cell, float frequencyModifier = 1)
        {
            if (Dungeon.Random.value() > feature.BaseFrequency * frequencyModifier)
                return false;

            if (!features.ContainsKey(cell))
                features.Add(cell, new List<DungeonFeature>());
            foreach (var f in features[cell])
                if (!f.MayStackWith(feature))
                    return false;
            features[cell].Add(feature);
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