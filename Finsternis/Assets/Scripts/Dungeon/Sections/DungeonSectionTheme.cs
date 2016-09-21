namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityQuery;

    public class DungeonSectionTheme : ScriptableObject
    {
        [SerializeField]
        private List<string> tags;

        [SerializeField]
        private List<GameObject> floorPrefabs;

        [SerializeField]
        private List<GameObject> wallPrefabs;

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

        public GameObject GetRandomWall()
        {
            return this.wallPrefabs[Dungeon.Random.IntRange(0, this.wallPrefabs.Count)];
        }

        public GameObject GetRandomFloor()
        {
            return this.floorPrefabs[Dungeon.Random.IntRange(0, this.floorPrefabs.Count)];
        }

    }
}