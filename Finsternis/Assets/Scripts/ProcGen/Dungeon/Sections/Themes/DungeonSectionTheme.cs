namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using Extensions;

    public abstract class DungeonSectionTheme : ScriptableObject
    {
        [SerializeField]
        private List<string> tags;

        [SerializeField]
        private List<GameObject> floorPrefabs;

        [SerializeField]
        private List<WallParts> walls;

        [SerializeField][Range(0, 2)]
        private float spawnDensityModifier = 1;

        public float SpawnDensityModifier { get { return this.spawnDensityModifier; } }

        public void AddTag(string tag)
        {
            tags.AddUnique(tag);
        }

        public void RemoveTag(string tag)
        {
            tags.Remove(tag);
        }

        public bool HasTag(string tag)
        {
            return this.tags.Contains(tag);
        }

        public bool HasTags(IEnumerable<string> tags)
        {
            return this.tags.ContainsAll(tags);
        }

        public bool HasAnyTag(IEnumerable<string> tags)
        {
            bool tagFound = false;
            foreach (var tag in tags)
            {
                if (HasTag(tag))
                {
                    tagFound = true;
                    break;
                }
            }
            return tagFound;
        }

        public WallParts GetRandomWall()
        {
            return GetRandomElement(this.walls);
        }

        public GameObject GetRandomFloor()
        {
            return GetRandomElement(this.floorPrefabs);
        }

        protected T GetRandomElement<T>(List<T> list)
        {
            return list.GetRandom(UnityEngine.Random.Range);
        }
    }
}