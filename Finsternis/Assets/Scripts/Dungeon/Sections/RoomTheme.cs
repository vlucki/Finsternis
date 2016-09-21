namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;

    public class RoomTheme : DungeonSectionTheme
    {
        [SerializeField]
        private List<GameObject> doorsPrefabs;

        public GameObject GetRandomDoor()
        {
            return this.doorsPrefabs[Dungeon.Random.IntRange(0, this.doorsPrefabs.Count)];
        }
    }
}