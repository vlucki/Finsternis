namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;

    public class CorridorTheme : DungeonSectionTheme
    {
        [SerializeField]
        private List<GameObject> trapsPrefabs;

        public GameObject GetRandomTrap()
        {
            return this.trapsPrefabs[Dungeon.Random.IntRange(0, this.trapsPrefabs.Count)];
        }
    }
}